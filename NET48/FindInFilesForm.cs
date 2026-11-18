using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindInFiles {
	public partial class FindInFilesForm : Form {
		private Process process = null;
		public FindInFilesForm(string[] args) {
			InitializeComponent();
			DarkMode.ApplyDarkTheme(this, contextMenuStrip);
			AcceptButton = buttonFind;
			richTextBox.AllowDrop = true;
			richTextBox.DragEnter += FindInFilesForm_DragEnter;
			richTextBox.DragDrop += FindInFilesForm_DragDrop;
			lineRender = new OutputLineRender(richTextBox);
			lineParser = new OutputLineParser(this, lineRender);
			var settings = AppSettings.Default;
			DarkMode.SetMenuChecked(saveSearchHistoryToolStripMenuItem, settings.SaveSearchHistory);
			comboBoxSearchPath.AddRange(settings.SearchPathHistory);
			comboBoxSearchPattern.AddRange(settings.SearchPatternHistory);
			textBoxEncoding.Text = defaultEncoding;
			var searchPath = false;
			for (var index = 0; index < args.Length; index++) {
				var arg = args[index];
				if (arg == "-E" && index + 1 < args.Length) {
					++index;
					textBoxEncoding.Text = args[index];
				} else if (Util.PathExists(arg).exist) {
					searchPath = true;
					comboBoxSearchPath.Text = arg;
					break;
				}
			}
			if (!searchPath) {
				lineRender.ShowComment($"drag & drop file or folder to search!{Environment.NewLine}");
			}
			var theme = settings.StyleTheme + 1;
			if (themeToolStripMenuItem.DropDownItems[theme] is ToolStripMenuItem menuItem) {
				menuItem.Checked = true;
			}
		}

		private async void buttonStart_Click(object sender, EventArgs e) {
			if (process != null) {
				StopFind();
				return;
			}

			var exePath = Util.FindExePath("rg.exe");
			if (!File.Exists(exePath)) {
				lineRender.ShowError($"ripgrep (rg.exe) not found!{Environment.NewLine}");
				return;
			}
			var searchPath = comboBoxSearchPath.Text.Trim();
			if (string.IsNullOrEmpty(searchPath)) {
				lineRender.ShowError($"drag & drop file or folder to search!{Environment.NewLine}");
				return;
			}
			var (exist, directory) = Util.PathExists(searchPath);
			if (!exist) {
				lineRender.ShowError($"file or folder \"{searchPath}\" not exists!{Environment.NewLine}");
				return;
			}
			var pattern = comboBoxSearchPattern.Text;
			if (string.IsNullOrEmpty(pattern)) {
				lineRender.ShowError($"empty search pattern!{Environment.NewLine}");
				return;
			}

			var argList = new List<string> {
				"--json --crlf"
			};
			lineParser.MaxLinesAfterMatch = 0;
			var text = textBoxContexLine.Text.Trim();
			if (text.Length != 0) {
				var lines = text.Split(',');
				int.TryParse(lines[0], out var before);
				if (lines.Length > 1) {
					int.TryParse(lines[1], out var after);
					if (before > 0) {
						argList.Add($"-B {before}");
					}
					if (after > 0) {
						argList.Add($"-A {after}");
						if (before > 0) {
							lineParser.MaxLinesAfterMatch = after;
						}
					}
				} else if (before > 0) {
					lineParser.MaxLinesAfterMatch = before;
					argList.Add($"-C {before}");
				}
			}
			if (!checkBoxRegex.Checked) {
				argList.Add("-F");
			}
			if (!checkBoxMatchCase.Checked) {
				argList.Add("-i");
			}
			if (checkBoxWholeWord.Checked) {
				argList.Add("-w");
			}
			if (checkBoxMultiline.Checked) {
				argList.Add("-U --multiline-dotall");
			}
			if (checkBoxPcre2.Checked) {
				argList.Add("-P");
			}
			if (checkBoxInvert.Checked) {
				argList.Add("-v");
			}
			text = textBoxEncoding.Text.Trim();
			if (!string.IsNullOrEmpty(text) && !text.Equals(defaultEncoding, StringComparison.OrdinalIgnoreCase)) {
				argList.Add($"-E \"{text.ToLowerInvariant()}\"");
			}
			if (directory) {
				if (!checkBoxRecursive.Checked) {
					argList.Add("-d 1");
				}
				var items = textBoxGlob.Text.Split(';');
				for (var i = 0; i < items.Length; i++) {
					var item = items[i].Trim();
					if (!string.IsNullOrEmpty(item) && item != "*.*") {
						argList.Add($"-g \"{item}\"");
					}
				}
			}
			argList.Add($"-e \"{pattern}\"");
			argList.Add($"\"{searchPath}\"");
			var argument = string.Join(" ", argList);
			ResetMatch();
			lineRender.ShowComment($"{argument}{Environment.NewLine}");

			buttonFind.Text = "S&top";
			try {
				process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
				process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
				process.StartInfo.FileName = exePath;
				process.StartInfo.Arguments = argument;
				process.OutputDataReceived += Process_OutputDataReceived;
				process.Start();
				process.BeginOutputReadLine();
				var error = await process.StandardError.ReadToEndAsync();
				if (process != null) {
					await Task.Run(() => process?.WaitForExit());
				}
				lineParser.Flush();
				if (lineParser.TotalMatchCount > 0) {
					SaveSearchHistory(searchPath, pattern);
				}
				if (!string.IsNullOrEmpty(error)) {
					lineRender.ShowError($"{error}{Environment.NewLine}");
				}
			} catch (Exception ex) {
				lineRender.ShowException("Start: ", ex);
			} finally {
				buttonFind.Text = "&Find All";
				process?.Dispose();
				process = null;
			}
		}

		private void StopFind() {
			buttonFind.Text = "&Find All";
			if (process != null && !process.HasExited) {
				try {
					process.Kill();
				} catch (Exception ex) {
					lineRender.ShowException("Stop: ", ex);
				} finally {
					process?.Dispose();
					process = null;
				}
			}
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			var data = e.Data;
			if (!string.IsNullOrEmpty(data)) {
				lineParser.Parse(data);
			}
		}

		private readonly OutputLineRender lineRender;
		private readonly OutputLineParser lineParser;
		private static readonly string defaultEncoding = "Unicode (UTF-8, UTF-16)";

		private void ResetMatch() {
			var page = lineRender.Reset();
			lineParser.Reset(page);
		}

		private void FindInFilesForm_DragEnter(object sender, DragEventArgs e) {
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void FindInFilesForm_DragDrop(object sender, DragEventArgs e) {
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0) {
					comboBoxSearchPath.Text = files[0];
				}
			}
		}

		private void richTextBox_DoubleClick(object sender, EventArgs e) {
			lineRender.StartEditor();
		}

		private void clearFindResultToolStripMenuItem_Click(object sender, EventArgs e) {
			lineRender.Clear();
		}

		private void selectFontToolStripMenuItem_Click(object sender, EventArgs e) {
			var font = lineRender.Font;
			fontDialog.Font = font;
			if (fontDialog.ShowDialog(this) == DialogResult.OK) {
				font = fontDialog.Font;
				AppSettings.Default.FindResultFont = font;
			}
			lineRender.Font = font;
		}

		private void fontDialog_Apply(object sender, EventArgs e) {
			lineRender.Font = fontDialog.Font;
		}

		private void FindInFilesForm_FormClosing(object sender, FormClosingEventArgs e) {
			StopFind();
			AppSettings.Default.Save();
		}

		private void SaveSearchHistory(string searchPath, string pattern) {
			comboBoxSearchPath.AddToTop(searchPath);
			comboBoxSearchPattern.AddToTop(pattern);
			var settings = AppSettings.Default;
			if (settings.SaveSearchHistory) {
				var count = AppSettings.MaxSearchHistoryCount;
				Util.AddHistory(settings.SearchPathHistory, searchPath, count);
				Util.AddHistory(settings.SearchPatternHistory, pattern, count);
			}
		}

		private void saveSearchHistoryToolStripMenuItem_Click(object sender, EventArgs e) {
			var settings = AppSettings.Default;
			var save = !settings.SaveSearchHistory;
			settings.SaveSearchHistory = save;
			DarkMode.SetMenuChecked(saveSearchHistoryToolStripMenuItem, save);
		}

		private void clearSearchHistoryToolStripMenuItem_Click(object sender, EventArgs e) {
			AppSettings.Default.ClearSearchHistory();
			comboBoxSearchPath.Items.Clear();
			comboBoxSearchPattern.Items.Clear();
		}

		private void followSystemToolStripMenuItem_Click(object sender, EventArgs e) {
			var index = -1;
			var value = -1;
			foreach (ToolStripMenuItem menuItem in themeToolStripMenuItem.DropDownItems) {
				if (menuItem == sender) {
					menuItem.Checked = true;
					value = index;
				} else {
					menuItem.Checked = false;
				}
				++index;
			}
			AppSettings.Default.StyleTheme = value;
		}
	}
}
