using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FindInFiles {
	internal static class Util {
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

		public static unsafe int GetCharIndex(string text, int startIndex, ref int byteCount, int bytePos) {
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

		public static void StartEditor(string path, int line) {
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

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		static extern int SearchPathW(string? lpPath, string lpFileName, string? lpExtension, int nBufferLength, char[] lpBuffer, IntPtr lpFilePart);
	}
}
