using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EasyHook;

namespace GitUI.Theming
{
    internal static class Win32ThemingHooks
    {
        private static Theme _theme;
        private static ColorDelegate _colorBypass;
        private static BrushDelegate _brushBypass;
        private static ThemeColorDelegate _themeColorBypass;
        private static ThemeBrushDelegate _themeBrushBypass;
        private static GetThemeColorDelegate _getThemeColorBypass;
        private static MessageBoxADelegate _messageBoxABypass;
        private static MessageBoxWDelegate _messageBoxWBypass;
        private static OpenThemeDataDelegate _openThemeDataBypass;
        private static OpenThemeDataExDelegate _openThemeDataExBypass;
        private static CloseThemeDataDelegate _closeThemeDataBypass;
        private static DrawThemeBackgroundDelegate _drawThemeBackgroundBypass;
        private static DrawThemeBackgroundExDelegate _drawThemeBackgroundExBypass;

        private static LocalHook _colorHook;
        private static LocalHook _brushHook;
        private static LocalHook _themeColorHook;
        private static LocalHook _themeBrushHook;
        private static LocalHook _getThemeColorHook;
        private static LocalHook _messageBoxAHook;
        private static LocalHook _messageBoxWHook;
        private static LocalHook _openThemeDataHook;
        private static LocalHook _openThemeDataExHook;
        private static LocalHook _closeThemeDataHook;
        private static LocalHook _drawThemeBackgroundHook;
        private static LocalHook _drawThemeBackgroundExHook;

        private static bool _showingMessageBox;
        private static readonly Dictionary<IntPtr, HashSet<string>> _classesByTheme =
            new Dictionary<IntPtr, HashSet<string>>();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int ColorDelegate(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr BrushDelegate(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int ThemeColorDelegate(IntPtr htheme, int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate IntPtr ThemeBrushDelegate(IntPtr htheme, int nIndex);

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
        private delegate int DrawThemeBackgroundDelegate(
            IntPtr hTheme, IntPtr hdc,
            int partId, int stateId,
            [In] Native.Struct.COMRECT pRect, [In] Native.Struct.COMRECT pClipRect);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int DrawThemeBackgroundExDelegate
        (IntPtr hTheme, IntPtr hdc,
            int partId, int stateId,
            Native.Struct.COMRECT pRect, ref Native.Struct.DTBGOPTS poptions);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate int GetThemeColorDelegate(IntPtr hTheme,
            int ipartId, int istateId, int ipropId,
            out int pColor);

        public static void Uninstall()
        {
            _colorHook?.Dispose();
            _brushHook?.Dispose();
            _themeColorHook?.Dispose();
            _themeBrushHook?.Dispose();
            _getThemeColorHook?.Dispose();
            _messageBoxAHook?.Dispose();
            _messageBoxWHook?.Dispose();

            _openThemeDataHook?.Dispose();
            _openThemeDataExHook?.Dispose();
            _closeThemeDataHook?.Dispose();
            _drawThemeBackgroundHook?.Dispose();
            _drawThemeBackgroundExHook?.Dispose();
        }

        public static void InstallColorHooks(Theme theme)
        {
//            (_brushHook, _brushBypass) = InstallHook<BrushDelegate>(
//                "user32.dll",
//                "GetSysColorBrush",
//                BrushHook);
//
//            (_colorHook, _colorBypass) = InstallHook<ColorDelegate>(
//                "user32.dll",
//                "GetSysColor",
//                ColorHook);

            (_themeBrushHook, _themeBrushBypass) = InstallHook<ThemeBrushDelegate>(
                "uxtheme.dll",
                "GetThemeSysColorBrush",
                ThemeBrushHook);

            (_themeColorHook, _themeColorBypass) = InstallHook<ThemeColorDelegate>(
                "uxtheme.dll",
                "GetThemeSysColor",
                ThemeColorHook);

            (_getThemeColorHook, _getThemeColorBypass) =
                InstallHook<GetThemeColorDelegate>(
                    "uxtheme.dll",
                    "GetThemeColor",
                    GetThemeColorHook);

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
                "OpenThemeDataEx",
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

            (_drawThemeBackgroundExHook, _drawThemeBackgroundExBypass) =
                InstallHook<DrawThemeBackgroundExDelegate>(
                    "uxtheme.dll",
                    "DrawThemeBackgroundEx",
                    DrawThemeBackgroundExHook);

            OpenThemeDataHook(IntPtr.Zero, "Scrollbar");
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

        private static int ThemeColorHook(IntPtr htheme, int nIndex)
        {
            Debug.WriteLine("ThemeColor " + htheme + " " + nIndex);
            if (_showingMessageBox)
            {
                return _themeColorBypass(htheme, nIndex);
            }

            var color = _theme.GetColor(GetKnownColor(nIndex));
            if (color == Color.Empty)
            {
                return _themeColorBypass(htheme, nIndex);
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

        private static int GetThemeColorHook(IntPtr htheme, int ipartid, int istateid, int ipropid, out int pcolor)
        {
            Debug.WriteLine($"GetThemeColor {htheme} {ipartid} {istateid} {(Native.Property)ipropid}");

            Native.Methods.GetThemeString(htheme,
                0, 0, (int)Native.Property.TMT_DISPLAYNAME,
                out StringBuilder themeName, 1000);

            Debug.WriteLine($"  {themeName}");

            KnownColor name;
            switch ((Native.Property)ipropid)
            {
                case Native.Property.TMT_WINDOW:
                    name = KnownColor.Window;
                    break;
                case Native.Property.TMT_WINDOWTEXT:
                case Native.Property.TMT_BODYTEXTCOLOR:
                    name = KnownColor.WindowText;
                    break;
                case Native.Property.TMT_SCROLLBAR:
                    name = KnownColor.ScrollBar;
                    break;
                case Native.Property.TMT_MENUTEXT:
                    name = KnownColor.MenuText;
                    break;
                case Native.Property.TMT_GRAYTEXT:
                    name = KnownColor.GrayText;
                    break;
                case Native.Property.TMT_BTNFACE:
                    name = KnownColor.Control;
                    break;
                case Native.Property.TMT_BTNSHADOW:
                    name = KnownColor.ControlDark;
                    break;
                case Native.Property.TMT_BTNHIGHLIGHT:
                    name = KnownColor.ControlLight;
                    break;
                case Native.Property.TMT_BTNTEXT:
                case Native.Property.TMT_TEXTCOLOR:
                    name = KnownColor.ControlText;
                    break;
                case Native.Property.TMT_WINDOWFRAME:
                    name = KnownColor.ActiveBorder;
                    break;
                case Native.Property.TMT_INACTIVEBORDER:
                    name = KnownColor.InactiveBorder;
                    break;
                case Native.Property.TMT_ACTIVEBORDER:
                    name = KnownColor.ActiveBorder;
                    break;
                case Native.Property.TMT_FILLCOLOR:
                    name = KnownColor.Control;
                    break;
                case Native.Property.TMT_BACKGROUND:
                    name = KnownColor.Control;
                    break;
                default:
                    name = 0;
                    break;
            }

            Debug.WriteLine($"  {name}");

            if (name == 0)
            {
                return _getThemeColorBypass(htheme, ipartid, istateid, ipropid, out pcolor);
            }

            var color = _theme.GetColor(name);
            if (color == Color.Empty)
            {
                return _getThemeColorBypass(htheme, ipartid, istateid, ipropid, out pcolor);
            }

            pcolor = ColorTranslator.ToWin32(color);
            return 1;
        }

        private static IntPtr ThemeBrushHook(IntPtr htheme, int nIndex)
        {
            Debug.WriteLine("ThemeBrush " + htheme + " " + nIndex);
            if (_showingMessageBox)
            {
                return _themeBrushBypass(htheme, nIndex);
            }

            var color = _theme.GetColor(GetKnownColor(nIndex));
            if (color == Color.Empty)
            {
                return _themeBrushBypass(htheme, nIndex);
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
            Debug.WriteLine("OpenThemeData " + classList);
            var htheme = _openThemeDataBypass(hwnd, classList);
            if (!_classesByTheme.TryGetValue(htheme, out var classes))
            {
                classes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classesByTheme.Add(htheme, classes);
            }

            classes.Add(classList);
            Debug.WriteLine("  " + htheme);
            return htheme;
        }

        private static IntPtr OpenThemeDataExHook(IntPtr hwnd, string classList, int dwflags)
        {
            Debug.WriteLine("OpenThemeDataEx " + classList);
            var htheme = _openThemeDataExBypass(hwnd, classList, dwflags);
            if (!_classesByTheme.TryGetValue(htheme, out var classes))
            {
                classes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _classesByTheme.Add(htheme, classes);
            }

            classes.Add(classList);
            Debug.WriteLine("  " + htheme);
            return htheme;
        }

        private static int CloseThemeDataHook(IntPtr htheme)
        {
            Debug.WriteLine("CloseThemeData " + htheme);
            return _closeThemeDataBypass(htheme);
        }

        private static int DrawThemeBackgroundHook(
            IntPtr htheme, IntPtr hdc,
            int partId, int stateId,
            Native.Struct.COMRECT prect, Native.Struct.COMRECT pcliprect)
        {
            Debug.WriteLine($"DrawThemeBackground {htheme} part {partId} state {stateId}");
            if (_classesByTheme.TryGetValue(htheme, out var classes))
            {
                if (classes.Contains("Scrollbar"))
                {
                    var g = Graphics.FromHdc(hdc);
                    g.ResetClip();
                    g.Clear(Color.Red);
                    for (int i = -10; i < 10; i++)
                    for (int j = -10; j < 10; j++)
                        g.DrawString(
                            $"{i}.{j}",
                            new Font(FontFamily.GenericMonospace, 6f),
                            new SolidBrush(Color.Yellow),
                            i * 20, j* 20);
                    g.Flush();
                    switch (partId)
                    {
                        case Native.ScrollBar.Parts.SBP_ARROWBTN:
                            g.FillRectangle(new SolidBrush(Color.Green), prect.ToRectangle());
                            break;

                        case Native.ScrollBar.Parts.SBP_THUMBBTNHORZ:
                        case Native.ScrollBar.Parts.SBP_THUMBBTNVERT:
                            g.FillRectangle(new SolidBrush(Color.Lime), prect.ToRectangle());
                            break;

                        case Native.ScrollBar.Parts.SBP_LOWERTRACKHORZ:
                        case Native.ScrollBar.Parts.SBP_LOWERTRACKVERT:
                        case Native.ScrollBar.Parts.SBP_UPPERTRACKHORZ:
                        case Native.ScrollBar.Parts.SBP_UPPERTRACKVERT:
                            g.FillRectangle(new SolidBrush(Color.Olive), prect.ToRectangle());
                            break;

                        case Native.ScrollBar.Parts.SBP_GRIPPERHORZ:
                        case Native.ScrollBar.Parts.SBP_GRIPPERVERT:
                        case Native.ScrollBar.Parts.SBP_SIZEBOX:
                            g.FillRectangle(new SolidBrush(Color.Magenta), prect.ToRectangle());
                            break;
                    }

                    g.Flush();
                    return 0;
                }
//                else if (classes.Contains("Edit"))
//                {
//                    switch (partId)
//                    {
//                        case Native.Edit.Parts.EP_EDITBORDER_HSCROLL:
//                        case Native.Edit.Parts.EP_EDITBORDER_HVSCROLL:
//                        case Native.Edit.Parts.EP_EDITBORDER_VSCROLL:
//                            g.Clear(Color.Magenta);
//                            return 1;
//                        case Native.Edit.Parts.EP_BACKGROUND:
//                        case Native.Edit.Parts.EP_CARET:
//                        case Native.Edit.Parts.EP_EDITTEXT:
//                        case Native.Edit.Parts.EP_PASSWORD:
//                        case Native.Edit.Parts.EP_BACKGROUNDWITHBORDER:
//                        case Native.Edit.Parts.EP_EDITBORDER_NOSCROLL:
//                        default:
//                            break;
//                    }
//                }
//                else if (classes.Contains("Button-OK;Button"))
//                {
//                    switch (partId)
//                    {
//                        case Native.Button.Parts.BP_PUSHBUTTON:
//                            switch (stateId)
//                            {
//                                case Native.Button.States.PushButton.PBS_HOT:
//                                    g.Clear(Color.Blue);
//                                    break;
//                                case Native.Button.States.PushButton.PBS_NORMAL:
//                                    g.Clear(Color.Orange);
//                                    break;
//                                case Native.Button.States.PushButton.PBS_PRESSED:
//                                    g.Clear(Color.Red);
//                                    break;
//                                case Native.Button.States.PushButton.PBS_DISABLED:
//                                    g.Clear(Color.Gray);
//                                    break;
//                                case Native.Button.States.PushButton.PBS_DEFAULTED:
//                                    g.Clear(Color.Magenta);
//                                    break;
//                                case Native.Button.States.PushButton.PBS_DEFAULTED_ANIMATING:
//                                    g.Clear(Color.Magenta);
//                                    break;
//                            }
//
//                            return 1;
//                        default:
//                            break;
//                    }
//                }
            }

            return _drawThemeBackgroundBypass(htheme, hdc, partId, stateId, prect, pcliprect);
        }

        private static int DrawThemeBackgroundExHook(IntPtr htheme, IntPtr hdc, int partId, int stateId, Native.Struct.COMRECT prect, ref Native.Struct.DTBGOPTS poptions)
        {
            Debug.WriteLine($"DrawThemeBackgroundEx {htheme} part {partId} state {stateId}");
            return _drawThemeBackgroundExBypass(htheme, hdc, partId, stateId, prect, ref poptions);
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
