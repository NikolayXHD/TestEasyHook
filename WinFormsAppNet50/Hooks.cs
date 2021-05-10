using System;
using System.Drawing;
using System.Runtime.InteropServices;
using EasyHook;

namespace WinFormsAppNet50
{
	internal static class Hooks
	{
		private static ColorDelegate _colorBypass;
		private static BrushDelegate _brushBypass;

		private static LocalHook _colorHook;
		private static LocalHook _brushHook;

		[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		private delegate int ColorDelegate(int nIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		private delegate IntPtr BrushDelegate(int nIndex);

		public static void InstallBrushHook()
		{
			(_brushHook, _brushBypass) = InstallHook<BrushDelegate>(
				"user32.dll",
				"GetSysColorBrush",
				BrushHook);
		}

		public static void InstallColorHook()
		{
			(_colorHook, _colorBypass) = InstallHook<ColorDelegate>(
				"user32.dll",
				"GetSysColor",
				ColorHook);
		}

		private static (LocalHook, TDelegate) InstallHook<TDelegate>(string dll, string method, TDelegate hookImpl)
			where TDelegate: Delegate
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
			return ColorTranslator.ToWin32(Color.Magenta);
		}

		private static IntPtr BrushHook(int nIndex)
		{
			return CreateSolidBrush(ColorTranslator.ToWin32(Color.Magenta));
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateSolidBrush(int nIndex);
	}
}
