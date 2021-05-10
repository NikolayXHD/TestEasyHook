using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

namespace WinFormsAppNet50
{
	internal static class Hooks
	{
		private static IHook<ColorDelegate> _colorHook;
		private static IHook<BrushDelegate> _brushHook;

		[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		private delegate int ColorDelegate(int nIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr BrushDelegate(int nIndex);

		public static void InstallBrushHook()
		{
			IntPtr libHandle = LoadLibrary("user32");
			IntPtr functionHandle = GetProcAddress(libHandle, "GetSysColorBrush");

			_brushHook = ReloadedHooks.Instance
				.CreateHook<BrushDelegate>(BrushHook, (long)functionHandle)
				.Activate();
		}

		public static void InstallColorHook()
		{
			IntPtr libHandle = LoadLibrary("user32");
			IntPtr functionHandle = GetProcAddress(libHandle, "GetSysColor");

			_colorHook = ReloadedHooks.Instance
				.CreateHook<ColorDelegate>(ColorHook, (long)functionHandle)
				.Activate();
		}

		private static int ColorHook(int nIndex)
		{
			var result = ColorTranslator.ToWin32(Color.Magenta);
			// var result = _colorHook.OriginalFunction.Invoke(nIndex);
			return result;
		}

		private static IntPtr BrushHook(int nIndex)
		{
			return CreateSolidBrush(ColorTranslator.ToWin32(Color.Magenta));
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateSolidBrush(int nIndex);

		[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		[DllImport("kernel32", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	}
}
