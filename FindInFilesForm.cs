using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FindInFiles {
	public partial class FindInFilesForm : Form {
		public FindInFilesForm(string[] args) {
			InitializeComponent();
			var font = Properties.Settings.Default.FindResultFont;
			if (font != null) {
				SetFindResultFont(font);
			}
			textBoxContexLine.Text = "0";
			checkBoxRecursive.Checked = true;
			checkBoxMatchCase.Checked = true;
			foreach (var arg in args) {
				if (Util.PathExists(arg)) {
					textBoxSearchPath.Text = arg;
					break;
				}
			}
		}

		private async void buttonStart_Click(object sender, EventArgs e) {
			var exePath = Util.FindExePath("rg.exe");
			if (!File.Exists(exePath)) {
				AppendText($"ripgrep (rg.exe) not found {Environment.NewLine}", Color.Red);
				return;
			}
			var searchPath = textBoxSearchPath.Text.Trim();
			if (!Util.PathExists(searchPath)) {
				return;
			}
			var pattern = textBoxPattern.Text;
			if (string.IsNullOrEmpty(pattern)) {
				return;
			}

			var argList = new List<string> {
				"--json --crlf"
			};
			var text = textBoxContexLine.Text.Trim();
			maxContextLine = 0;
			if (int.TryParse(text, out int line) && line > 0) {
				maxContextLine = line;
				argList.Add($"-C {line}");
			}
			if (!checkBoxRecursive.Checked) {
				argList.Add("-d 1");
			}
			if (!checkBoxMatchCase.Checked) {
				argList.Add("-i");
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
			argList.Add($"-e \"{pattern}\"");
			argList.Add($"\"{searchPath}\"");
			var argument = string.Join(" ", argList);
			ResetMatchResult();
			AppendText($"{argument}{Environment.NewLine}", Color.Gray);

			using (var process = new Process()) {
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
				await process.WaitForExitAsync();
				if (!string.IsNullOrEmpty(error)) {
					AppendText($"{error}{Environment.NewLine}", Color.Red);
				}
			}
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			var data = e.Data;
			if (!string.IsNullOrEmpty(data)) {
				ProcessOutputLine(data);
			}
		}

		private int maxContextLine = 0;
		private int contextLineCount = 0;
		private bool afterMatch = false;
		private static readonly string MarkerPath = "\u200C"; // zero width non-joiner
		private static readonly string MarkerLine = "\u200D"; // zero width joiner

		private void ResetMatchResult() {
			richTextBox.Clear();
			contextLineCount = 0;
			afterMatch = false;
		}

		private void ProcessOutputLine(string line) {
			var root = JsonDocument.Parse(line).RootElement;
			if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("type", out var type) && root.TryGetProperty("data", out var data)) {
				var dataType = type.GetString();
				if (dataType != "context") {
					contextLineCount = 0;
				}
				if (dataType == "begin") {
					afterMatch = false;
					var path = data.GetProperty("path").GetProperty("text").GetString();
					Invoke(AppendText, $"{MarkerPath}{path}{Environment.NewLine}", Color.Blue);
				} else if (dataType == "match") {
					afterMatch = true;
					var text = data.GetProperty("lines").GetProperty("text").GetString();
					var number = data.GetProperty("line_number").GetInt32();
					var matches = data.GetProperty("submatches");
					Invoke(AddMatchLine, number, text, matches);
				} else if (dataType == "context") {
					if (maxContextLine != 0 && afterMatch) {
						++contextLineCount;
						if (contextLineCount > maxContextLine) {
							afterMatch = false;
							Invoke(AppendText, $"--{Environment.NewLine}", Color.Gray);
						}
					}
					var text = data.GetProperty("lines").GetProperty("text").GetString();
					var number = data.GetProperty("line_number").GetInt32();
					Invoke(AddContextLine, number, text);
				} else if (dataType == "end" || dataType == "summary") {
					var stats = data.GetProperty("stats");
					var matched_lines = stats.GetProperty("matched_lines").GetInt32();
					var matches = stats.GetProperty("matches").GetInt32();
					var elapsed = stats.GetProperty("elapsed").GetProperty("human").GetString();
					var summary = $"matched lines: {matched_lines}, matches: {matches}, elapsed: {elapsed}";
					if (dataType == "end") {
						var path = data.GetProperty("path").GetProperty("text").GetString();
						path = Path.GetFileName(path);
						summary = $"-- {path}, {summary}{Environment.NewLine}";
					} else {
						var elapsed_total = data.GetProperty("elapsed_total").GetProperty("human").GetString();
						summary = $"-- total {summary}, total elapsed: {elapsed_total}{Environment.NewLine}";
					}
					Invoke(AppendText, summary, Color.Gray);
				}
			}
		}

		private void AddContextLine(int number, string? line) {
			AppendText($"{number}{MarkerLine}-", Color.Olive);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
		}

		private void AddMatchLine(int number, string? line, JsonElement matches) {
			AppendText($"{number}{MarkerLine}:", Color.Green);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			var docOffset = richTextBox.TextLength + padding.Length;
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
			if (line.Length == 0) {
				return;
			}
			var count = matches.GetArrayLength();
			if (count == 0) { // invert
				return;
			}
			var ascii = Util.GetLeadingAsciiCount(line);
			var startIndex = ascii;
			var byteCount = ascii;
			for (var index = 0; index < count;) {
				var match = matches[index++];
				var start = match.GetProperty("start").GetInt32();
				var end = match.GetProperty("end").GetInt32() - start;
				var text = match.GetProperty("match").GetProperty("text").GetString();
				var space = false;
				if (!string.IsNullOrEmpty(text)) {
					space = char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[^1]);
					// convert byte offset to character index
					end = text.Length;
					if (start > ascii) {
						//start = line.IndexOf(text, startIndex, StringComparison.Ordinal);
						//startIndex = start + end;
						start = Util.GetCharacterIndex(line, startIndex, ref byteCount, start);
						if (index < count) {
							startIndex = start + end;
							byteCount += Util.GetUTF8ByteCount(text);
						}
					}
				}
				richTextBox.Select(start + docOffset, end);
				richTextBox.SelectionColor = Color.Red;
				if (space) {
					richTextBox.SelectionBackColor = Color.Green;
				}
			}
		}

		private void AppendText(string text, Color color) {
			richTextBox.SelectionStart = richTextBox.TextLength;
			richTextBox.SelectionColor = color;
			richTextBox.SelectedText = text;
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
					textBoxSearchPath.Text = files[0];
				}
			}
		}

		private void richTextBox_DoubleClick(object sender, EventArgs e) {
			// line number is wrong when word wrap is enabled
			var start = richTextBox.GetFirstCharIndexOfCurrentLine();
			var lineno = richTextBox.GetLineFromCharIndex(start);
			if (lineno < 1) {
				return;
			}
			var end = richTextBox.GetFirstCharIndexFromLine(lineno + 1);
			end = richTextBox.Find(MarkerLine, start, end, RichTextBoxFinds.MatchCase | RichTextBoxFinds.NoHighlight);
			if (end > start) {
				var text = richTextBox.GetTextRange(start, end);
				if (int.TryParse(text, out var num)) {
					var column = start + text.Length + 2;
					start = richTextBox.Find(MarkerPath, 0, start, RichTextBoxFinds.MatchCase | RichTextBoxFinds.Reverse | RichTextBoxFinds.NoHighlight);
					if (start >= 0) {
						++start;
						lineno = richTextBox.GetLineFromCharIndex(start);
						end = richTextBox.GetFirstCharIndexFromLine(lineno + 1);
						text = richTextBox.GetTextRange(start, end).Trim();
						column = richTextBox.SelectionStart - column;
						Util.StartEditor(text, num, column);
					}
				}
			}
		}

		private void clearAllToolStripMenuItem_Click(object sender, EventArgs e) {
			richTextBox.Clear();
		}

		private void selectFontToolStripMenuItem_Click(object sender, EventArgs e) {
			var font = richTextBox.Font;
			fontDialog.Font = font;
			if (fontDialog.ShowDialog(this) == DialogResult.OK) {
				font = fontDialog.Font;
				Properties.Settings.Default.FindResultFont = font;
				Properties.Settings.Default.Save();
			}
			SetFindResultFont(font);
		}

		private void SetFindResultFont(Font font) {
			var current = richTextBox.Font;
			if (font == current || (font.Name == current.Name && font.Style == current.Style && font.Size == current.Size)) {
				return; // highlighting is lost when change font
			}
			richTextBox.Font = font;
		}

		private void fontDialog_Apply(object sender, EventArgs e) {
			SetFindResultFont(fontDialog.Font);
		}
	}
}
