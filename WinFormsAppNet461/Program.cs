using System;
using System.Windows.Forms;

namespace WinFormsAppNet461
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Hooks.Install();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
