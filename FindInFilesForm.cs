using System.Diagnostics;
using System.Runtime.InteropServices;
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
				if (Path.Exists(arg)) {
					textBoxSearchPath.Text = arg;
					break;
				}
			}
		}

		private async void buttonStart_Click(object sender, EventArgs e) {
			var exePath = FindExePath("rg.exe");
			if (!File.Exists(exePath)) {
				AppendText($"ripgrep (rg.exe) not found {Environment.NewLine}", Color.Red);
				return;
			}
			var searchPath = textBoxSearchPath.Text.Trim();
			if (!Path.Exists(searchPath)) {
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
					var text = data.GetProperty("lines").GetProperty("text").GetString()?.TrimEnd('\r', '\n');
					var number = data.GetProperty("line_number").GetInt32().ToString();
					var matches = data.GetProperty("submatches");
					afterMatch = true;
					Invoke(AddMatchLine, number, text, matches);
				} else if (dataType == "context") {
					if (maxContextLine != 0 && afterMatch) {
						++contextLineCount;
						if (contextLineCount > maxContextLine) {
							afterMatch = false;
							Invoke(AppendText, $"--{Environment.NewLine}", Color.Gray);
						}
					}
					var text = data.GetProperty("lines").GetProperty("text").GetString()?.TrimEnd('\r', '\n');
					var number = data.GetProperty("line_number").GetInt32().ToString();
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

		private void AddContextLine(string number, string? line) {
			AppendText($"{number}{MarkerLine}-", Color.Olive);
			var padding = string.IsNullOrEmpty(line) ? "" : " ";
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
		}

		private void AddMatchLine(string number, string? line, JsonElement matches) {
			AppendText($"{number}{MarkerLine}:", Color.Green);
			var padding = string.IsNullOrEmpty(line) ? "" : " ";
			var docOffset = richTextBox.TextLength + padding.Length;
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
			if (string.IsNullOrEmpty(line)) {
				return;
			}
			var count = matches.GetArrayLength();
			if (count == 0) { // invert
				return;
			}
			var ascii = GetLeadingAsciiCount(line);
			var startIndex = ascii;
			for (var index = 0; index < count; index++) {
				var match = matches[index];
				var start = match.GetProperty("start").GetInt32();
				var end = match.GetProperty("end").GetInt32() - start;
				var text = match.GetProperty("match").GetProperty("text").GetString();
				var space = false;
				if (!string.IsNullOrEmpty(text)) {
					space = char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[^1]);
					// convert byte offset to character index
					end = text.Length;
					if (start > ascii) {
						start = line.IndexOf(text, startIndex);
						startIndex += end;
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
				var selStart = richTextBox.SelectionStart;
				var selLength = richTextBox.SelectionLength;
				richTextBox.Select(start, end - start);
				var text = richTextBox.SelectedText;
				if (int.TryParse(text, out var num)) {
					start = richTextBox.Find(MarkerPath, 0, start, RichTextBoxFinds.MatchCase | RichTextBoxFinds.Reverse | RichTextBoxFinds.NoHighlight);
					if (start >= 0) {
						++start;
						lineno = richTextBox.GetLineFromCharIndex(start);
						end = richTextBox.GetFirstCharIndexFromLine(lineno + 1);
						richTextBox.Select(start, end - start);
						text = richTextBox.SelectedText.Trim();
						StartEditor(text, num);
					}
				}
				richTextBox.Select(selStart, selLength);
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

		private static bool IsSameFont(Font font, Font other) {
			return font == other || (font.Name == other.Name && font.Style == other.Style && font.Size == other.Size);
		}

		private static void StartEditor(string path, int line) {
			var exePath = FindExePath("Notepad2.exe");
			if (!File.Exists(exePath)) {
				return;
			}
			var startInfo = new ProcessStartInfo {
				UseShellExecute = false,
				FileName = exePath,
				Arguments = $"/g {line} \"{path}\"",
			};
			using var process = Process.Start(startInfo);
		}

		private static unsafe int GetLeadingAsciiCount(string text) {
			var count = 0;
			fixed (char* ptr = text) {
				char* p = ptr;
				char* end = p + text.Length;
				while (p < end && *p < 0x80) {
					++p;
					++count;
				}
			}
			return count;
		}

		private static string FindExePath(string name) {
			var path = Path.Combine(Application.StartupPath, name);
			if (File.Exists(path)) {
				return path;
			}
			char[] buffer = new char[260];
			int length = SearchPathW(null, name, null, buffer.Length, buffer, 0);
			if (length > 0 && length < buffer.Length) {
				name = new string(buffer, 0, length);
			}
			return name;
		}

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int SearchPathW(string? lpPath, string lpFileName, string? lpExtension, int nBufferLength, char[] lpBuffer, IntPtr lpFilePart);
	}
}
