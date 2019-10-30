using System.Drawing;
using System.Runtime.InteropServices;

namespace GitUI.Theming
{
	[StructLayout(LayoutKind.Sequential)]
	public class COMRECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public COMRECT()
		{
		}

		public COMRECT(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public Rectangle ToRectangle() =>
			Rectangle.FromLTRB(Left, Top, Right, Bottom);

		public override string ToString() =>
			$"({Left},{Top}) -> ({Right},{Bottom})";
	}
}
