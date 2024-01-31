using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FindInFiles {
	internal static class Util {
		public static string RemoveLineEnding(string? line) {
			if (string.IsNullOrEmpty(line)) {
				return string.Empty;
			}
			var length = line.Length;
			var ch = line[length - 1];
			if (ch == '\n' || ch == '\r') {
				length -= 1;
				if (ch == '\n' && length > 0 && line[length - 1] == '\r') {
					length -= 1;
				}
				line = (length == 0) ? string.Empty : line[..length];
			}
			return line;
		}

		public static unsafe int GetLeadingAsciiCount(string text) {
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

		public static unsafe int GetUTF8ByteCount(string text) {
			var count = 0;
			fixed (char* ptr = text) {
				char* p = ptr;
				char* end = p + text.Length;
				while (p < end) {
					var ch = *p++;
					if (ch < 0x80) {
						count += 1;
					} else if (ch < 0x800) {
						count += 2;
					} else if (p < end && char.IsSurrogatePair(ch, *p)) {
						++p;
						count += 4;
					} else {
						count += 3;
					}
				}
			}
			return count;
		}

		public static unsafe int GetCharacterIndex(string text, int startIndex, ref int byteCount, int bytePos) {
			var count = byteCount;
			fixed (char* ptr = text) {
				char* p = ptr + startIndex;
				char* end = p + text.Length;
				while (p < end && count < bytePos) {
					var ch = *p++;
					++startIndex;
					if (ch < 0x80) {
						count += 1;
					} else if (ch < 0x800) {
						count += 2;
					} else if (p < end && char.IsSurrogatePair(ch, *p)) {
						++p;
						++startIndex;
						count += 4;
					} else {
						count += 3;
					}
				}
			}
			byteCount = count;
			return startIndex;
		}

		public static void StartEditor(string path, int line, int column) {
			var exePath = FindExePath("Notepad2.exe");
			if (!File.Exists(exePath)) {
				return;
			}
			var startInfo = new ProcessStartInfo {
				UseShellExecute = false,
				FileName = exePath,
				Arguments = $"/g {line},{column} \"{path}\"",
			};
			using var process = Process.Start(startInfo);
		}

		public static string FindExePath(string name) {
			var path = Path.Combine(Application.StartupPath, name);
			if (File.Exists(path)) {
				return path;
			}
			char[] buffer = new char[260];
			int length = SearchPathW(null, name, null, buffer.Length, buffer, IntPtr.Zero);
			if (length > 0 && length < buffer.Length) {
				name = new string(buffer, 0, length);
			}
			return name;
		}

		public static (bool exist, bool directory) PathExists(string? path) {
			if (!string.IsNullOrEmpty(path)) {
				var attr = GetFileAttributesW(path);
				if (attr != INVALID_FILE_ATTRIBUTES) {
					var directory = (attr & FILE_ATTRIBUTE_DIRECTORY) != 0;
					return (true, directory);
				}
			}
			return (false, false);
		}

		private const int INVALID_FILE_ATTRIBUTES = -1;
		private const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int GetFileAttributesW(string lpFileName);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int SearchPathW(string? lpPath, string lpFileName, string? lpExtension, int nBufferLength, char[] lpBuffer, IntPtr lpFilePart);

		public static unsafe string GetTextRange(this TextBoxBase textBox, int start, int end) {
			var buffer = new char[end - start];
			fixed (char* ptr = buffer) {
				TEXTRANGE range;
				range.chrg.cpMin = start;
				range.chrg.cpMax = end;
				range.lpstrText = new IntPtr(ptr);
				int length = SendMessage(textBox.Handle, EM_GETTEXTRANGE, IntPtr.Zero, range);
				if (length > 0) {
					return new string(buffer, 0, length);
				}
			}
			return string.Empty;
		}

		public static float GetLineHeight(this TextBoxBase textBox) {
			var font = textBox.Font;
			var family = font.FontFamily;
			var style = font.Style;
			var emHeight = family.GetEmHeight(style);
			var lineSpacing = family.GetLineSpacing(style);
			var ascent = family.GetCellAscent(style);
			var descent = family.GetCellDescent(style);
			var height = font.Size * (lineSpacing + (ascent + descent) / 2) / emHeight;
			return height;
		}

		public static void SetRedraw(this Control control, bool redraw) {
			SendMessage(control.Handle, WM_SETREDRAW, new IntPtr(redraw ? 1 : 0), IntPtr.Zero);
		}

		private const int WM_SETREDRAW = 0x000B;
		private const int WM_USER = 0x0400;
		private const int EM_GETTEXTRANGE = WM_USER + 75;

		[StructLayout(LayoutKind.Sequential)]
		private struct CHARRANGE {
			public int cpMin;
			public int cpMax;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct TEXTRANGE {
			public CHARRANGE chrg;
			public IntPtr lpstrText;
		}

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, TEXTRANGE lParam);
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
	}
}
