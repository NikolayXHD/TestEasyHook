using System;
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
			Win32ThemingHooks.InstallThemeHooks();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			Win32ThemingHooks.Uninstall();
		}
	}
}
