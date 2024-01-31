using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FindInFiles {
	public partial class FindInFilesForm : Form {
		public FindInFilesForm(string[] args) {
			InitializeComponent();
			lineParser = new OutputLineParser(this, RenderOutput);
			var font = Properties.Settings.Default.FindResultFont;
			if (font != null) {
				SetFindResultFont(font);
			}
			richTextBox.AllowDrop = true;
			richTextBox.DragEnter += FindInFilesForm_DragEnter;
			richTextBox.DragDrop += FindInFilesForm_DragDrop;
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
			lineParser.MaxContextLine = 0;
			if (int.TryParse(text, out int line) && line > 0) {
				lineParser.MaxContextLine = line;
				argList.Add($"-C {line}");
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
			if (!checkBoxRecursive.Checked) {
				argList.Add("-d 1");
			}
			if (checkBoxInvert.Checked) {
				argList.Add("-v");
			}
			argList.Add($"-e \"{pattern}\"");
			argList.Add($"\"{searchPath}\"");
			var argument = string.Join(" ", argList);
			ClearMatchResult();
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
				lineParser.Flush();
				richTextBox.Invalidate();
				if (!string.IsNullOrEmpty(error)) {
					AppendText($"{error}{Environment.NewLine}", Color.Red);
				}
			}
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			var data = e.Data;
			if (!string.IsNullOrEmpty(data)) {
				lineParser.Parse(data);
			}
		}

		private readonly OutputLineParser lineParser;
		private static readonly string MarkerPath = "\u200C"; // zero width non-joiner
		private static readonly string MarkerLine = "\u200D"; // zero width joiner
		private int visibleDelta = 0;

		private void ClearMatchResult() {
			var page = (int)(richTextBox.Height / richTextBox.GetLineHeight());
			page = Math.Max(page, 5);
			visibleDelta = page * 2;
			richTextBox.Clear();
			lineParser.MaxCachedLine = page;
			lineParser.Clear();
		}

		private void RenderOutput(List<OutputLine> lines) {
			var visible = richTextBox.Focused;
			if (!visible) {
				var last = richTextBox.GetLineFromCharIndex(-1);
				if (last < visibleDelta) {
					visible = true;
				} else {
					var top = richTextBox.GetLineFromCharIndex(richTextBox.GetCharIndexFromPosition(new Point(1, 1)));
					visible = last - top < visibleDelta;
				}
			}
			richTextBox.SetRedraw(false);
			foreach (var line in lines) {
				switch (line.LineType) {
				case OutputLineType.Path:
					AppendText($"{MarkerPath}{line.Text}{Environment.NewLine}", Color.Blue);
					break;

				case OutputLineType.Match:
					AddMatchLine(line.Number, line.Text, line.Matches);
					break;

				case OutputLineType.Context:
					AddContextLine(line.Number, line.Text);
					break;

				default:
					AppendText($"{line.Text}{Environment.NewLine}", Color.Gray);
					break;
				}
			}
			richTextBox.SetRedraw(true);
			if (visible) {
				richTextBox.Invalidate();
			}
		}

		private void AddContextLine(int number, string? line) {
			AppendText($"{number}{MarkerLine}-", Color.Olive);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
		}

		private void AddMatchLine(int number, string? line, JsonElement? submatches) {
			AppendText($"{number}{MarkerLine}:", Color.Green);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			var docOffset = richTextBox.TextLength + padding.Length;
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
			if (line.Length == 0 || submatches == null) {
				return;
			}
			var matches = submatches.Value;
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

		private void FindInFilesForm_DragEnter(object? sender, DragEventArgs e) {
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void FindInFilesForm_DragDrop(object? sender, DragEventArgs e) {
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
