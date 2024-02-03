using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FindInFiles {
	internal sealed class OutputLineRender {
		private readonly RichTextBox richTextBox;

		private static readonly string MarkerPath = "\u200C"; // zero width non-joiner
		private static readonly string MarkerLine = "\u200D"; // zero width joiner
		private int visibleDelta = 0;

		public OutputLineRender(RichTextBox richTextBox) {
			this.richTextBox = richTextBox;
		}

		public Font Font {
			get { return richTextBox.Font; }
			set {
				var current = richTextBox.Font;
				if (value == current || (value.Name == current.Name && value.Style == current.Style && value.Size == current.Size)) {
					return; // highlighting is lost when change font
				}
				richTextBox.Font = value;
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

		private void AddContextLine(string number, string line) {
			AppendText($"{number}{MarkerLine}-", Color.Olive);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
		}

		private void AddMatchLine(string number, string line, MatchTextRange[] matches) {
			AppendText($"{number}{MarkerLine}:", Color.Green);
			line = Util.RemoveLineEnding(line);
			var padding = (line.Length == 0) ? "" : " ";
			var docOffset = richTextBox.TextLength + padding.Length;
			AppendText($"{padding}{line}{Environment.NewLine}", SystemColors.WindowText);
			if (matches == null) {
				return;
			}
			foreach (var match in matches) {
				richTextBox.Select(match.Start + docOffset, match.Length);
				richTextBox.SelectionColor = Color.Red;
				if (match.Space) {
					richTextBox.SelectionBackColor = Color.Green;
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
	}
}
