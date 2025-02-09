using System.Text;
using System.Text.Json;

namespace FindInFiles {
	internal sealed class OutputLineParser {
		private readonly Control owner;
		private readonly OutputLineRender render;
		private readonly List<OutputLine> cachedLines = new();

		public int MaxLinesAfterMatch = 0;
		public int MaxCachedLine = 0;
		public int TotalMatchCount = 0;
		private int linesAfterMatch = 0;
		private bool afterMatch = false;

		public OutputLineParser(Control owner, OutputLineRender render) {
			this.owner = owner;
			this.render = render;
		}

		public void Clear() {
			TotalMatchCount = 0;
			linesAfterMatch = 0;
			afterMatch = false;
			cachedLines.Clear();
		}

		public void Reset(int cache) {
			Clear();
			MaxCachedLine = cache;
		}

		public void Flush() {
			if (cachedLines.Count != 0) {
				RenderOutput();
			}
			render.Invalidate();
		}

		private void RenderOutput() {
			render.RenderOutput(cachedLines);
			cachedLines.Clear();
		}

		private static string? GetText(JsonElement element) {
			string? text;
			if (element.TryGetProperty("text", out var prop)) {
				text = prop.GetString();
			} else {
				text = element.GetProperty("bytes").GetString();
				var bytes = Convert.FromBase64String(text ?? "");
				text = Encoding.UTF8.GetString(bytes);
			}
			return text;
		}

		public void Parse(string line) {
			var root = JsonDocument.Parse(line).RootElement;
			if (root.ValueKind != JsonValueKind.Object) {
				return;
			}
			if (!root.TryGetProperty("type", out var type) || !root.TryGetProperty("data", out var data)) {
				return;
			}
			OutputLine outputLine;
			var dataType = type.GetString();
			switch (dataType) {
			case "begin": {
				var path = data.GetProperty("path").GetProperty("text").GetString();
				outputLine = new OutputLine { LineType = OutputLineType.Path, Text = path };
			} break;

			case "match": {
				var text = GetText(data.GetProperty("lines"));
				var number = data.GetProperty("line_number").GetInt32();
				var submatches = data.GetProperty("submatches");
				var matches = ParseSubMatches(text, submatches);
				outputLine = new OutputLine { LineType = OutputLineType.Match, Text = text, Number = number, Matches = matches };
			} break;

			case "context": {
				var text = GetText(data.GetProperty("lines"));
				var number = data.GetProperty("line_number").GetInt32();
				outputLine = new OutputLine { LineType = OutputLineType.Context, Text = text, Number = number };
			} break;

			case "end":
			case "summary": {
				var stats = data.GetProperty("stats");
				var matched_lines = stats.GetProperty("matched_lines").GetInt32();
				var matches = stats.GetProperty("matches").GetInt32();
				var elapsed = stats.GetProperty("elapsed").GetProperty("human").GetString();
				var summary = $"matched lines: {matched_lines}, matches: {matches}, elapsed: {elapsed}";
				if (dataType == "end") {
					var path = data.GetProperty("path").GetProperty("text").GetString();
					path = Path.GetFileName(path);
					summary = $"-- {path}, {summary}";
				} else {
					TotalMatchCount = matches;
					var elapsed_total = data.GetProperty("elapsed_total").GetProperty("human").GetString();
					summary = $"-- total {summary}, total elapsed: {elapsed_total}";
				}
				outputLine = new OutputLine { LineType = OutputLineType.Summary, Text = summary };
			} break;

			default:
				return;
			}

			if (MaxLinesAfterMatch != 0) {
				if (outputLine.LineType == OutputLineType.Context) {
					if (afterMatch) {
						++linesAfterMatch;
						if (linesAfterMatch > MaxLinesAfterMatch) {
							afterMatch = false;
							cachedLines.Add(new OutputLine { LineType = OutputLineType.Separator, Text = "--" });
						}
					}
				} else {
					linesAfterMatch = 0;
					afterMatch = outputLine.LineType == OutputLineType.Match;
				}
			}
			cachedLines.Add(outputLine);
			if (cachedLines.Count >= MaxCachedLine) {
				owner.Invoke(RenderOutput);
			}
		}

		private static MatchTextRange[]? ParseSubMatches(string? line, JsonElement submatches) {
			if (string.IsNullOrEmpty(line) || submatches.ValueKind != JsonValueKind.Array) {
				return null;
			}
			var count = submatches.GetArrayLength();
			if (count == 0) { // invert
				return null;
			}
			var matches = new MatchTextRange[count];
			var ascii = Util.GetLeadingAsciiCount(line);
			var startIndex = ascii;
			var byteCount = ascii;
			for (var index = 0; index < count;) {
				ref var range = ref matches[index];
				var match = submatches[index++];
				var start = match.GetProperty("start").GetInt32();
				var end = match.GetProperty("end").GetInt32() - start;
				var text = match.GetProperty("match").GetProperty("text").GetString();
				if (!string.IsNullOrEmpty(text)) {
					end = text.Length;
					range.Space = char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[end - 1]);
					// convert byte offset to character index
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
				range.Start = start;
				range.Length = end;
			}
			return matches;
		}
	}

	internal struct MatchTextRange {
		public int Start;
		public int Length;
		public bool Space;
	}

	internal enum OutputLineType {
		Path,
		Match,
		Context,
		Separator,
		Summary,
	}

	internal sealed class OutputLine {
		public OutputLineType LineType;
		public int Number;
		public string? Text;
		public MatchTextRange[]? Matches;
	}
}
