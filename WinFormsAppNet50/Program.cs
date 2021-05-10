using System;
using System.Windows.Forms;

namespace WinFormsAppNet50
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// seems to work fine
			// Hooks.InstallBrushHook();

			// causes System.ExecutionEngineException
			// when creating Win32 window for Form1
			Hooks.InstallColorHook();

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.Run(new Form1());
		}
	}
}
