using System;
using System.Windows.Forms;

namespace FindInFiles {
	internal static class Program {
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.Run(new FindInFilesForm(args));
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			var exc = e.ExceptionObject as Exception;
			MessageBox.Show(exc?.StackTrace, exc?.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
