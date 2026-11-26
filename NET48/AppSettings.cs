using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FindInFiles {
	internal class AppSettings {
		private static readonly AppSettings defaultInstance = new AppSettings();
		public static AppSettings Default {
			get { return defaultInstance; }
		}

		private readonly string settingsPath;
		private readonly bool loaded = false;

		private readonly Font DefaultFont;
		public Font FindResultFont = null;
		public int StyleTheme = -1;

		public bool SaveSearchHistory = false;
		public const int MaxSearchHistoryCount = 32;
		public readonly List<string> SearchPathHistory = new List<string>();
		public readonly List<string> SearchPatternHistory = new List<string>();

		public void ClearSearchHistory() {
			SearchPathHistory.Clear();
			SearchPatternHistory.Clear();
		}

		public void Save() {
			try {
				DoSave();
			} catch (Exception) { }
		}

		private AppSettings() {
			var folder = Application.StartupPath;
			var path = Path.Combine(folder, "Notepad4.ini");
			if (!File.Exists(path)) {
				folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				folder = Path.Combine(folder, "Notepad4");
			}
			settingsPath = Path.Combine(folder, "FindInFiles.ini");
			loaded = File.Exists(settingsPath);
			if (loaded) {
				try {
					DoLoad();
				} catch (Exception) { }
			}
			try {
				var fontName = "Cascadia Mono";
				var font = new Font(fontName, 11);
				if (font.Name != fontName) {
					font = new Font("Consolas", 11);
				}
				DefaultFont = font;
				if (FindResultFont == null) {
					FindResultFont = font;
				}
			} catch (Exception) { }
		}

		private void DoLoad() {
			var document = File.ReadAllText(settingsPath, Encoding.UTF8);
			if (string.IsNullOrEmpty(document)) {
				return;
			}
			var separator = new char[] { '\r', '\n' };
			var lines = document.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines) {
				var index = line.IndexOf('=');
				if (index <= 4 || index + 1 == line.Length) {
					continue;
				}
				var key = line.Substring(0, index);
				var value = line.Substring(index + 1);
				switch (key) {
				case "FindResultFont": {
						var converter = TypeDescriptor.GetConverter(typeof(Font));
						FindResultFont = converter.ConvertFromString(value) as Font;
					}
					break;
				case "StyleTheme":
					if (int.TryParse(value, out var number) && number >= 0) {
						StyleTheme = (number > 1)? 1 : number;
					}
					break;
				case "SaveSearchHistory":
					if (int.TryParse(value, out number) && number != 0) {
						SaveSearchHistory = true;
					}
					break;
				default:
					if (key.StartsWith("Path_")) {
						SearchPathHistory.Add(value);
					} else if (key.StartsWith("Pattern_")) {
						SearchPatternHistory.Add(value);
					}
					break;
				}
			}
		}

		private void DoSave() {
			var section = "[FindInFiles]";
			var eol = Environment.NewLine;
			var builder = new StringBuilder(section);
			builder.Append(eol);
			if (FindResultFont != null && !FindResultFont.Equals(DefaultFont)) {
				var converter = TypeDescriptor.GetConverter(typeof(Font));
				var value = converter.ConvertToString(FindResultFont);
				builder.AppendFormat("FindResultFont={0}{1}", value, eol);
			}
			if (StyleTheme >= 0) {
				builder.AppendFormat("StyleTheme={0}{1}", StyleTheme, eol);
			}

			if (SaveSearchHistory) {
				builder.AppendFormat("SaveSearchHistory=1{0}", eol);
				var index = 0;
				foreach (var value in SearchPathHistory) {
					index += 1;
					builder.AppendFormat("Path_{0}={1}{2}", index, value, eol);
				}
				index = 0;
				foreach (var value in SearchPatternHistory) {
					index += 1;
					builder.AppendFormat("Pattern_{0}={1}{2}", index, value, eol);
				}
			}

			var empty = builder.Length == section.Length + eol.Length;
			if (!loaded && empty) {
				return;
			}
			if (!File.Exists(settingsPath)) {
				if (empty) {
					return;
				}
				var folder = Path.GetDirectoryName(settingsPath);
				if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
					Directory.CreateDirectory(folder);
				}
			}
			var content = builder.ToString();
			File.WriteAllText(settingsPath, content, Encoding.UTF8);
		}
	}
}
