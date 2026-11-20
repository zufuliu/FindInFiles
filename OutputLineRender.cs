namespace FindInFiles {
	internal sealed class OutputLineRender {
		private readonly RichTextBox richTextBox;

		private static readonly string MarkerPath = "\u200C"; // zero width non-joiner
		private static readonly string MarkerLine = "\u200D"; // zero width joiner
		private int visibleDelta = 0;

		private readonly Color TextColor = SystemColors.WindowText;
		private readonly Color ErrorTextColor = Color.Red;
		private readonly Color CommentTextColor = Color.Gray;
		private readonly Color PathTextColor = Color.Blue;
		private readonly Color MatchTextColor = Color.Red;

		public OutputLineRender(RichTextBox richTextBox) {
			this.richTextBox = richTextBox;
			var font = AppSettings.Default.FindResultFont;
			if (font != null) {
				richTextBox.Font = font;
				richTextBox.SelectionFont = font;
			}
			if (DarkMode.OutputUseDarkTheme) {
				if (!DarkMode.UseSystemColorMode) {
					richTextBox.BackColor = DarkMode.WindowColor;
					richTextBox.ForeColor = TextColor = DarkMode.WindowTextColor;
				}
				MatchTextColor = ErrorTextColor = Color.FromArgb(0xED, 0x1C, 0x24);
				PathTextColor = Color.FromArgb(0x00, 0x78, 0xD7);
			}
		}

		public Font Font {
			get { return richTextBox.Font; }
			set {
				var current = richTextBox.Font;
				if (value == current || (value.Name == current.Name && value.Style == current.Style && value.Size == current.Size)) {
					return; // highlighting is lost when change font
				}
				richTextBox.Font = value;
				richTextBox.SelectionFont = value;
			}
		}

		public void Invalidate() {
			richTextBox.Invalidate();
		}

		public void Clear() {
			richTextBox.Clear();
		}

		public int Reset() {
			var page = (int)(richTextBox.Height / richTextBox.GetLineHeight());
			page = Math.Max(page, 5);
			visibleDelta = page * 2;
			richTextBox.Clear();
			return page;
		}

		public void AppendText(string text, Color color) {
			richTextBox.SelectionStart = richTextBox.TextLength;
			richTextBox.SelectionColor = color;
			richTextBox.SelectedText = text;
		}

		public void ShowError(string message) {
			AppendText(message, ErrorTextColor);
		}
		public void ShowComment(string message) {
			AppendText(message, CommentTextColor);
		}
		public void ShowException(string tag, Exception ex) {
			AppendText($"{Environment.NewLine}{tag}{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}", ErrorTextColor);
		}

		public void RenderOutput(IList<OutputLine> lines) {
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
					AppendText($"{MarkerPath}{line.Text}{Environment.NewLine}", PathTextColor);
					break;

				case OutputLineType.Match:
					AddMatchLine(line.Number, line.Text, line.Matches);
					break;

				case OutputLineType.Context:
					AddContextLine(line.Number, line.Text);
					break;

				default:
					AppendText($"{line.Text}{Environment.NewLine}", CommentTextColor);
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
			AppendText($"{padding}{line}{Environment.NewLine}", TextColor);
		}

		private void AddMatchLine(int number, string? line, MatchTextRange[]? matches) {
			AppendText($"{number}{MarkerLine}:", Color.Green);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			var docOffset = richTextBox.TextLength + padding.Length;
			AppendText($"{padding}{line}{Environment.NewLine}", TextColor);
			if (matches == null) {
				return;
			}
			foreach (var match in matches) {
				richTextBox.Select(match.Start + docOffset, match.Length);
				richTextBox.SelectionColor = MatchTextColor;
				if (match.Space) {
					richTextBox.SelectionBackColor = Color.OliveDrab;
				}
			}
		}

		public void StartEditor() {
			// line number is wrong when word wrap is enabled
			var start = richTextBox.GetFirstCharIndexOfCurrentLine();
			var lineno = richTextBox.GetLineFromCharIndex(start);
			if (lineno < 1) {
				return;
			}
			var lineEnd = richTextBox.GetFirstCharIndexFromLine(lineno + 1);
			var end = richTextBox.Find(MarkerLine, start, lineEnd, RichTextBoxFinds.MatchCase | RichTextBoxFinds.NoHighlight);
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
			} else {
				end = richTextBox.Find(MarkerPath, start, lineEnd, RichTextBoxFinds.MatchCase | RichTextBoxFinds.NoHighlight);
				if (end >= start) {
					var text = richTextBox.GetTextRange(end + MarkerPath.Length, lineEnd).Trim();
					Util.StartEditor(text, 0, 0);
				}
			}
		}
	}
}
