using System;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
	static class NativeMethods
	{
		[DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
		public static extern int DrawThemeText(
			HandleRef hTheme,
			HandleRef hdc,
			int iPartId,
			int iStateId,
			[MarshalAs(UnmanagedType.LPWStr)] string pszText,
			int iCharCount,
			int dwTextFlags,
			int dwTextFlags2,
			[In] COMRECT pRect);

		[DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
		public static extern int DrawThemeBackground(
			HandleRef hTheme,
			HandleRef hdc,
			int partId,
			int stateId,
			[In] NativeMethods.COMRECT pRect,
			[In] NativeMethods.COMRECT pClipRect);

		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr OpenThemeData(IntPtr hWnd, String classList);

		[DllImport("uxtheme.dll")]
		public extern static Int32 CloseThemeData(IntPtr hTheme);

		[StructLayout(LayoutKind.Sequential)]
		public class COMRECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public COMRECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public override string ToString()
			{
				return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
			}
		}
	}
}
