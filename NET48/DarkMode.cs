using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FindInFiles {
	internal static class DarkMode {
		public static bool OutputUseDarkTheme = false;
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
			} catch (Exception) { }
			return false;
		}

		private static void SetAppDarkMode() {
			try {
				const int win10Build1809 = 17763;
				const int win11Build24H2 = 26100;
				const int win11Build25H2 = 26200;
				var version = new OSVERSIONINFOW();
				version.dwOSVersionInfoSize = Marshal.SizeOf(version);
				RtlGetVersion(ref version);
				if (version.dwMajorVersion == 10 && version.dwMinorVersion == 0 && version.dwBuildNumber >= win10Build1809) {
					var uxtheme = GetModuleHandleW("uxtheme.dll");
					var ord135 = GetProcAddress(uxtheme, new IntPtr(135));
					var ord136 = GetProcAddress(uxtheme, new IntPtr(136));
					if (ord135 != IntPtr.Zero && ord136 != IntPtr.Zero) {
						var setPreferredAppMode = Marshal.GetDelegateForFunctionPointer<SetPreferredAppMode>(ord135);
						var flushMenuThemes = Marshal.GetDelegateForFunctionPointer<FlushMenuThemes>(ord136);
						setPreferredAppMode(1);
						flushMenuThemes();
						buildNumber = version.dwBuildNumber;
						if (buildNumber > win11Build25H2) {
							supportDarkTheme = true;
						}
						else if (buildNumber >= win11Build24H2) {
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
			contextMenu.ShowImageMargin = false;
			if (buildNumber != 0) {
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

		// https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/wdm/ns-wdm-_osversioninfow
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct OSVERSIONINFOW {
			public int dwOSVersionInfoSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst =128)]
			public string szCSDVersion;
		}
		[DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern int RtlGetVersion(ref OSVERSIONINFOW versionInfo);

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
