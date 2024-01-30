using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FindInFiles {
	internal class OutputLineParser {
		private readonly Control owner;
		private readonly Action<List<OutputLine>> callback;
		private readonly List<OutputLine> cachedLines = new List<OutputLine>();

		public int MaxContextLine = 0;
		public int MaxCachedLine = 0;
		private int contextLineCount = 0;
		private bool afterMatch = false;

		public OutputLineParser(Control owner, Action<List<OutputLine>> callback) {
			this.owner = owner;
			this.callback = callback;
		}

		public void Clear() {
			contextLineCount = 0;
			afterMatch = false;
			cachedLines.Clear();
		}

		public void Flush() {
			if (cachedLines.Count != 0) {
				owner.Invoke(callback, cachedLines);
				cachedLines.Clear();
			}
		}

		public void Parse(string line) {
			var parser = new JsonParser();
			var root = parser.Parse(line);
			if (root == null || root.ValueType != JsonValueType.Object) {
				return;
			}
			if (!root.TryGetValue("type", out var type) || !root.TryGetValue("data", out var data)) {
				return;
			}
			OutputLine outputLine;
			var dataType = type.GetString();
			if (dataType == "begin") {
				var path = data["path"]["text"].GetString();
				outputLine = new OutputLine { LineType = OutputLineType.Path, Text = path };
			} else if (dataType == "match") {
				var text = data["lines"]["text"].GetString();
				var number = data["line_number"].GetString();
				var matches = data["submatches"];
				outputLine = new OutputLine { LineType = OutputLineType.Match, Text = text, Number = number, Matches = matches };
			} else if (dataType == "context") {
				var text = data["lines"]["text"].GetString();
				var number = data["line_number"].GetString();
				outputLine = new OutputLine { LineType = OutputLineType.Context, Text = text, Number = number };
			} else if (dataType == "end" || dataType == "summary") {
				var stats = data["stats"];
				var matched_lines = stats["matched_lines"].GetString();
				var matches = stats["matches"].GetString();
				var elapsed = stats["elapsed"]["human"].GetString();
				var summary = $"matched lines: {matched_lines}, matches: {matches}, elapsed: {elapsed}";
				if (dataType == "end") {
					var path = data["path"]["text"].GetString();
					path = Path.GetFileName(path);
					summary = $"-- {path}, {summary}";
				} else {
					var elapsed_total = data["elapsed_total"]["human"].GetString();
					summary = $"-- total {summary}, total elapsed: {elapsed_total}";
				}
				outputLine = new OutputLine { LineType = OutputLineType.Summary, Text = summary };
			} else {
				return;
			}
			if (MaxContextLine != 0) {
				if (outputLine.LineType == OutputLineType.Context) {
					if (afterMatch) {
						++contextLineCount;
						if (contextLineCount > MaxContextLine) {
							afterMatch = false;
							cachedLines.Add(new OutputLine { LineType = OutputLineType.Separator, Text = "--" });
						}
					}
				} else {
					contextLineCount = 0;
					afterMatch = outputLine.LineType == OutputLineType.Match;
				}
			}
			cachedLines.Add(outputLine);
			if (cachedLines.Count >= MaxCachedLine) {
				owner.Invoke(callback, cachedLines);
				cachedLines.Clear();
			}
		}
	}

	internal enum OutputLineType {
		Path,
		Match,
		Context,
		Separator,
		Summary,
	}

	internal class OutputLine {
		public OutputLineType LineType;
		public string Text;
		public string Number;
		public JsonValue Matches;
	}
}
