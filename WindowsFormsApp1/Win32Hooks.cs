using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using EasyHook;

namespace GitUI.Theming
{
    internal static class Win32ThemingHooks
    {
        private static Theme _theme;
        private static ColorDelegate _colorBypass;
        private static BrushDelegate _brushBypass;
        private static MessageBoxADelegate _messageBoxABypass;
        private static MessageBoxWDelegate _messageBoxWBypass;
        private static OpenThemeDataDelegate _openThemeDataBypass;
        private static OpenThemeDataExDelegate _openThemeDataExBypass;
        private static CloseThemeDataDelegate _closeThemeDataBypass;
        private static DrawThemeBackgroundDelegate _drawThemeBackgroundBypass;

        private static LocalHook _colorHook;
        private static LocalHook _brushHook;
        private static LocalHook _messageBoxAHook;
        private static LocalHook _messageBoxWHook;
        private static LocalHook _openThemeDataHook;
        private static LocalHook _openThemeDataExHook;
        private static LocalHook _closeThemeDataHook;
        private static LocalHook _drawThemeBackgroundHook;

        private static bool _showingMessageBox;
        private static readonly Dictionary<IntPtr, HashSet<string>> _classesByTheme =
            new Dictionary<IntPtr, HashSet<string>>();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int ColorDelegate(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr BrushDelegate(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        private delegate int MessageBoxADelegate(IntPtr hwnd, string test, string caption, int type);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private delegate int MessageBoxWDelegate(IntPtr hwnd, string test, string caption, int type);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private delegate IntPtr OpenThemeDataDelegate(IntPtr hWnd, String classList);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private delegate IntPtr OpenThemeDataExDelegate(IntPtr hWnd, String classList, int dwflags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int CloseThemeDataDelegate(IntPtr hTheme);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int DrawThemeBackgroundDelegate(IntPtr hTheme, IntPtr hdc,
            int partId, int stateId,
            [In] COMRECT pRect, [In] COMRECT pClipRect);

        public static void InstallColorHooks(Theme theme)
        {
            (_brushHook, _brushBypass) = InstallHook<BrushDelegate>(
                "user32.dll",
                "GetSysColorBrush",
                BrushHook);

            (_colorHook, _colorBypass) = InstallHook<ColorDelegate>(
                "user32.dll",
                "GetSysColor",
                ColorHook);

            _theme = theme;
        }

        public static void InstallMessageBoxHooks()
        {
            (_messageBoxAHook, _messageBoxABypass) = InstallHook<MessageBoxADelegate>(
                "user32.dll",
                "MessageBoxA",
                MessageBoxAHook);

            (_messageBoxWHook, _messageBoxWBypass) = InstallHook<MessageBoxWDelegate>(
                "user32.dll",
                "MessageBoxW",
                MessageBoxWHook);
        }

        public static void InstallThemeHooks()
        {
            (_openThemeDataHook, _openThemeDataBypass) = InstallHook<OpenThemeDataDelegate>(
                "uxtheme.dll",
                "OpenThemeData",
                OpenThemeDataHook);

            (_openThemeDataExHook, _openThemeDataExBypass) = InstallHook<OpenThemeDataExDelegate>(
                "uxtheme.dll",
                "OpenThemeData",
                OpenThemeDataExHook);

            (_closeThemeDataHook, _closeThemeDataBypass) = InstallHook<CloseThemeDataDelegate>(
                "uxtheme.dll",
                "CloseThemeData",
                CloseThemeDataHook);

            (_drawThemeBackgroundHook, _drawThemeBackgroundBypass) =
                InstallHook<DrawThemeBackgroundDelegate>(
                    "uxtheme.dll",
                    "DrawThemeBackground",
                    DrawThemeBackgroundHook);
        }

        public static void Uninstall()
        {
            _colorHook?.Dispose();
            _brushHook?.Dispose();
            _messageBoxAHook?.Dispose();
            _messageBoxWHook?.Dispose();

            _openThemeDataHook?.Dispose();
            _closeThemeDataHook?.Dispose();
            _drawThemeBackgroundHook?.Dispose();
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateSolidBrush(int nIndex);

        private static (LocalHook, TDelegate) InstallHook<TDelegate>(string dll, string method, TDelegate hookImpl)
            where TDelegate : Delegate
        {
            var addr = LocalHook.GetProcAddress(dll, method);
            var original = Marshal.GetDelegateForFunctionPointer<TDelegate>(addr);
            var hook = LocalHook.Create(addr, hookImpl, null);

            try
            {
                hook.ThreadACL.SetExclusiveACL(new int[0]);
            }
            catch
            {
                hook.Dispose();
                throw;
            }

            return (hook, original);
        }

        private static int ColorHook(int nIndex)
        {
            if (_showingMessageBox)
            {
                return _colorBypass(nIndex);
            }

            var color = _theme.GetColor(GetKnownColor(nIndex));
            if (color == Color.Empty)
            {
                return _colorBypass(nIndex);
            }

            return ColorTranslator.ToWin32(color);
        }

        private static IntPtr BrushHook(int nIndex)
        {
            if (_showingMessageBox)
            {
                return _brushBypass(nIndex);
            }

            var color = _theme.GetColor(GetKnownColor(nIndex));
            if (color == Color.Empty)
            {
                return _brushBypass(nIndex);
            }

            return CreateSolidBrush(ColorTranslator.ToWin32(color));
        }

        private static int MessageBoxAHook(IntPtr hwnd, string text, string caption, int type)
        {
            _showingMessageBox = true;
            var result = _messageBoxABypass(hwnd, text, caption, type);
            _showingMessageBox = false;
            return result;
        }

        private static int MessageBoxWHook(IntPtr hwnd, string text, string caption, int type)
        {
            _showingMessageBox = true;
            var result = _messageBoxWBypass(hwnd, text, caption, type);
            _showingMessageBox = false;
            return result;
        }

        private static IntPtr OpenThemeDataHook(IntPtr hwnd, string classList)
        {
            var htheme = _openThemeDataBypass(hwnd, classList);
            if (!_classesByTheme.TryGetValue(htheme, out var classes))
            {
                classes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classesByTheme.Add(htheme, classes);
            }

            classes.Add(classList);
            return htheme;
        }

        private static IntPtr OpenThemeDataExHook(IntPtr hwnd, string classList, int dwflags)
        {
            var htheme = _openThemeDataExBypass(hwnd, classList, dwflags);
            if (!_classesByTheme.TryGetValue(htheme, out var classes))
            {
                classes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classesByTheme.Add(htheme, classes);
            }

            classes.Add(classList);
            return htheme;
        }

        private static int CloseThemeDataHook(IntPtr htheme)
        {
            _classesByTheme.Remove(htheme);
            return _closeThemeDataBypass(htheme);
        }

        private static int DrawThemeBackgroundHook(IntPtr htheme, IntPtr hdc, int partId, int stateId, COMRECT prect, COMRECT pcliprect)
        {
            if (_classesByTheme.TryGetValue(htheme, out var classes))
            {
                var g = Graphics.FromHdc(hdc);
                if (pcliprect != null)
                {
                    g.SetClip(pcliprect.ToRectangle());
                }

                if (classes.Contains("Scrollbar"))
                {
                    switch (partId)
                    {
                        case Native.ScrollBar.Parts.SBP_ARROWBTN:
                            g.Clear(Color.Green);
                            return 1;

                        case Native.ScrollBar.Parts.SBP_THUMBBTNHORZ:
                        case Native.ScrollBar.Parts.SBP_THUMBBTNVERT:
                            g.Clear(Color.Lime);
                            return 1;

                        case Native.ScrollBar.Parts.SBP_LOWERTRACKHORZ:
                        case Native.ScrollBar.Parts.SBP_LOWERTRACKVERT:
                        case Native.ScrollBar.Parts.SBP_UPPERTRACKHORZ:
                        case Native.ScrollBar.Parts.SBP_UPPERTRACKVERT:
                            g.Clear(Color.LightBlue);
                            return 1;

                        case Native.ScrollBar.Parts.SBP_GRIPPERHORZ:
                        case Native.ScrollBar.Parts.SBP_GRIPPERVERT:
                        case Native.ScrollBar.Parts.SBP_SIZEBOX:
                        default:
                            break;
                    }
                }
                else if (classes.Contains("Edit"))
                {
                    switch (partId)
                    {
                        case Native.Edit.Parts.EP_EDITBORDER_HSCROLL:
                        case Native.Edit.Parts.EP_EDITBORDER_HVSCROLL:
                        case Native.Edit.Parts.EP_EDITBORDER_VSCROLL:
                            g.Clear(Color.Magenta);
                            return 1;
                        case Native.Edit.Parts.EP_BACKGROUND:
                        case Native.Edit.Parts.EP_CARET:
                        case Native.Edit.Parts.EP_EDITTEXT:
                        case Native.Edit.Parts.EP_PASSWORD:
                        case Native.Edit.Parts.EP_BACKGROUNDWITHBORDER:
                        case Native.Edit.Parts.EP_EDITBORDER_NOSCROLL:
                        default:
                            break;
                    }
                }
            }

            return _drawThemeBackgroundBypass(htheme, hdc, partId, stateId, prect, pcliprect);
        }

        private static KnownColor GetKnownColor(int nIndex)
        {
            if ((nIndex & 0xffffff00) == 0)
            {
                nIndex |= -0x80000000;
                return ColorTranslator.FromOle(nIndex).ToKnownColor();
            }

            return ColorTranslator.FromWin32(nIndex).ToKnownColor();
        }
    }
}
