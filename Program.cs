namespace FindInFiles {
	internal static class Program {
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.Run(new FindInFilesForm(args));
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			var exc = e.ExceptionObject as Exception;
			MessageBox.Show(exc?.StackTrace, exc?.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
