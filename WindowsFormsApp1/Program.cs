using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GitUI.Theming
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Win32ThemingHooks.InstallColorHooks(new StaticTheme(new Dictionary<KnownColor, Color>
			{
				[KnownColor.Window] = Color.Yellow,
				[KnownColor.Control] = Color.Yellow,

				[KnownColor.WindowText] = Color.Red,
				[KnownColor.ControlText] = Color.Red,

				[KnownColor.GrayText] = Color.Brown,

				[KnownColor.ScrollBar] = Color.Green,
				[KnownColor.ActiveBorder] = Color.Magenta,
				[KnownColor.InactiveBorder] = Color.DarkMagenta,
			}));

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			Win32ThemingHooks.Uninstall();
		}
	}
}
