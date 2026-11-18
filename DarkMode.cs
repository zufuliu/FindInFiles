using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace FindInFiles {
	internal static class DarkMode {
		public static bool OutputUseDarkTheme = false;
		public static bool UseSystemColorMode = false;
		private static bool supportDarkTheme = false;
		private static int buildNumber = 0;

		public static readonly Color WindowColor = Color.FromArgb(0x2E, 0x2E, 0x2E);
		public static readonly Color WindowTextColor = Color.FromArgb(0xF0, 0xF0, 0xF0);
		private static readonly Color ControlColor = Color.FromArgb(0x20, 0x20, 0x20);
		private static readonly Color MenuColor = Color.FromArgb(0x2F, 0x2F, 0x2F);

		public static void InitDarkMode() {
			if (IsAppsUseDarkTheme()) {
				OutputUseDarkTheme = true;
				SetAppDarkMode();
#if NET10_0_OR_GREATER
				if (buildNumber >= win10Build2004) {
					UseSystemColorMode = true;
					// SystemColorMode.System not work on Win10
					Application.SetColorMode(SystemColorMode.Dark);
				}
#endif
			}
		}

		private static bool IsAppsUseDarkTheme() {
			try {
				var theme = AppSettings.Default.StyleTheme;
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
			} catch (Exception) { }
			return false;
		}

		private static void SetAppDarkMode() {
			try {
				const int win10Build1809 = 17763;
				const int win11Build24H2 = 26100;
				const int win11Build25H2 = 26200;
				var version = Environment.OSVersion.Version;
				if (version.Major == 10 && version.Minor == 0 && version.Build >= win10Build1809) {
					var uxtheme = GetModuleHandleW("uxtheme.dll");
					var ord135 = GetProcAddress(uxtheme, new IntPtr(135));
					var ord136 = GetProcAddress(uxtheme, new IntPtr(136));
					if (ord135 != IntPtr.Zero && ord136 != IntPtr.Zero) {
						var setPreferredAppMode = Marshal.GetDelegateForFunctionPointer<SetPreferredAppMode>(ord135);
						var flushMenuThemes = Marshal.GetDelegateForFunctionPointer<FlushMenuThemes>(ord136);
						setPreferredAppMode(1);
						flushMenuThemes();
						buildNumber = version.Build;
						if (buildNumber > win11Build25H2) {
							supportDarkTheme = true;
						} else if (buildNumber >= win11Build24H2) {
							const int minWin11Revision = 6899; // 2025-10-14
							try {
								var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
								if (key != null) {
									var value = key.GetValue("UBR");
									if (value != null && (int)value >= minWin11Revision) {
										supportDarkTheme = true;
									}
								}
							}
							catch (Exception) { }
						}
					}
				}
			} catch (Exception) { }
		}

		public static void ApplyDarkTheme(Form form, ContextMenuStrip contextMenu) {
			if (buildNumber == 0) {
				return;
			}
			contextMenu.ShowImageMargin = false;
			if (UseSystemColorMode) {
				FixDarkTheme(form.Controls);
			} else {
				if (buildNumber >= win10Build2004) {
					int dark = 1;
					DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, 4);
				}
				form.BackColor = ControlColor;
				form.ForeColor = WindowTextColor;
				ApplyDarkTheme(form.Controls);
				contextMenu.ForeColor = WindowTextColor;
				contextMenu.BackColor = MenuColor;
				ApplyDarkColor(contextMenu.Items);
			}
		}

		private static void FixDarkTheme(Control.ControlCollection collection) {
			foreach (Control control in collection) {
				string? theme = null;
				if (supportDarkTheme) {
					if (control is not Label) {
						theme = "DarkMode_DarkTheme";
					}
				} else {
					if ((control is TextBox textBox) && !textBox.Multiline) {
						theme = "DarkMode_CFD";
					}
				}
				if (theme != null) {
					SetWindowTheme(control.Handle, theme, IntPtr.Zero);
				}
				if (control.HasChildren) {
					FixDarkTheme(control.Controls);
				}
			}
		}

		private static void ApplyDarkTheme(Control.ControlCollection collection) {
			var primaryTheme = supportDarkTheme ? "DarkMode_DarkTheme" : "DarkMode_Explorer";
			foreach (Control control in collection) {
				var theme = primaryTheme;
				var back = WindowColor;
				if (control is Label) {
					back = ControlColor;
					theme = null;
				} else if (control is CheckBox) {
					back = ControlColor;
				} else if (!supportDarkTheme && ((control is ComboBox) || ((control is TextBox textBox) && !textBox.Multiline))) {
					theme = "DarkMode_CFD";
				}
				if (theme != null) {
					SetWindowTheme(control.Handle, theme, IntPtr.Zero);
				}
				control.BackColor = back;
				control.ForeColor = WindowTextColor;
				if (control.HasChildren) {
					ApplyDarkTheme(control.Controls);
				}
			}
		}

		private static void ApplyDarkColor(ToolStripItemCollection collection) {
			foreach (var item in collection) {
				if (item is ToolStripMenuItem menuItem) {
					menuItem.ForeColor = WindowTextColor;
					menuItem.BackColor = MenuColor;
					if (menuItem.HasDropDownItems) {
						ApplyDarkColor(menuItem.DropDownItems);
					}
				}
			}
		}

		public static void SetMenuChecked(ToolStripMenuItem menuItem, bool isChecked) {
			menuItem.Checked = isChecked;
			if (buildNumber != 0) {
				menuItem.ForeColor = isChecked ? Color.Green : (UseSystemColorMode ? SystemColors.WindowText : WindowTextColor);
			}
		}

		private const int win10Build2004 = 19041;
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private static extern IntPtr GetModuleHandleW(string lpModuleName);
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr lpProcName);
		private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
		[DllImport("uxtheme.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, IntPtr pszSubIdList);
		[DllImport("dwmapi.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
		private delegate int SetPreferredAppMode(int mode);
		private delegate void FlushMenuThemes();
	}
}
