using Microsoft.Win32;

namespace FindInFiles {
	internal static class DarkMode {
		public static bool OutputUseDarkTheme = false;

		public static readonly Color WindowColor = Color.FromArgb(0x2E, 0x2E, 0x2E);
		public static readonly Color WindowTextColor = Color.FromArgb(0xF0, 0xF0, 0xF0);

		public static void InitDarkMode() {
			if (IsAppsUseDarkTheme()) {
				OutputUseDarkTheme = true;
#if NET10_0_OR_GREATER
				// SystemColorMode.System not work on Win10
				Application.SetColorMode(SystemColorMode.Dark);
#endif
			}
		}

		private static bool IsAppsUseDarkTheme() {
			try {
				var theme = Properties.Settings.Default.StyleTheme;
				if (theme == 0) {
					return false;
				}
				if (theme > 0) {
					return true;
				}
				var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
				if (key != null) {
					var value = key.GetValue("AppsUseLightTheme");
					if (value != null && (int)value == 0) {
						if (SystemInformation.HighContrast) {
							return false;
						}
						return true;
					}
				}
			}
			catch (Exception) { }
			return false;
		}
	}
}
