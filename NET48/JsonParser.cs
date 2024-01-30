using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FindInFiles {
	internal class JsonParser {
		private readonly StringBuilder builder = new StringBuilder();

		public unsafe JsonValue Parse(string json) {
			fixed (char* ptr = json) {
				var p = ptr;
				var end = ptr + json.Length;
				var next = p;
				return ParseValue(p, end, ref next);
			}
		}

		private unsafe JsonValue ParseValue(char* p, char* end, ref char* next) {
			while (p < end) {
				var ch = *p++;
				switch (ch) {
				case '[':
					return ParseArray(p, end, ref next);

				case '{':
					return ParseObject(p, end, ref next);

				case '\"': {
						var str = ScanString(p, end, ref next);
						if (str == null) {
							return null;
						}
						return new JsonValue(str);
					}

				default:
					if (ch > ' ') {
						if (IsNumChar(ch)) {
							var t = p;
							while (t < end && IsNumChar(*t)) {
								++t;
							}
							next = t;
							var num = new string(p - 1, 0, (int)(t - p + 1));
							return new JsonValue(num, JsonValueType.Number);
						}
						return null;
					}
					break;
				}
			}
			return null;
		}

		private unsafe JsonValue ParseArray(char* p, char* end, ref char* next) {
			var list = new List<JsonValue>();
			var hasValue = false;
			while (p < end) {
				var ch = *p++;
				switch (ch) {
				case ',':
					if (hasValue) {
						hasValue = false;
					} else {
						return null;
					}
					break;

				case ']':
					next = p;
					return new JsonValue(list);

				default:
					if (ch > ' ') {
						var value = ParseValue(p - 1, end, ref next);
						if (value == null) {
							return null;
						}
						hasValue = true;
						p = next;
						list.Add(value);
					}
					break;
				}
			}
			return null;
		}

		private unsafe JsonValue ParseObject(char* p, char* end, ref char* next) {
			var dict = new Dictionary<string, JsonValue>();
			var hasValue = false;
			while (p < end) {
				var ch = *p++;
				switch (ch) {
				case ',':
					if (hasValue) {
						hasValue = false;
					} else {
						return null;
					}
					break;

				case '}':
					next = p;
					return new JsonValue(dict);

				default:
					if (ch == '\"' && !hasValue) {
						var key = ScanString(p, end, ref next);
						if (key == null) {
							return null;
						}
						p = next;
						while (p < end) {
							ch = *p++;
							if (ch == ':') {
								var value = ParseValue(p, end, ref next);
								if (value == null) {
									return null;
								}
								hasValue = true;
								p = next;
								dict[key] = value;
								break;
							}
							if (ch > ' ') {
								return null;
							}
						}
					} else if (ch > ' ') {
						return null;
					}
					break;
				}
			}
			return null;
		}

		private unsafe string ScanString(char* p, char* end, ref char* next) {
			builder.Clear();
			var start = p;
			var stop = p;
			while (p < end) {
				var ch = *p++;
				if (ch == '\"') {
					next = p;
					var trail = (int)(stop - start);
					if (builder.Length == 0) {
						if (trail == 0) {
							return string.Empty;
						}
						return new string(start, 0, trail);
					}
					if (trail != 0) {
						builder.Append(start, trail);
					}
					return builder.ToString();
				}
				if (ch != '\\' || p == end) {
					++stop;
				} else {
					if (start != stop) {
						builder.Append(start, (int)(stop - start));
					}
					var chNext = *p++;
					switch(chNext) {
					case '\"':
					case '\\':
					case '/':
						break;
					case 'b':
						chNext = '\b';
						break;
					case '\f':
						chNext = '\f';
						break;
					case 'n':
						chNext = '\n';
						break;
					case 'r':
						chNext = '\r';
						break;
					case 't':
						chNext = '\t';
						break;
					case 'u': {
							int digit = 0;
							int code = 0;
							var t = p;
							while (t < end && digit < 4) {
								ch = *t++;
								var hex = GetHexDigit(ch);
								if (hex < 0) {
									break;
								}
								++digit;
								code = (code << 4) | hex;
							}
							if (digit == 4) {
								p = t;
								chNext = (char)code;
							} else {
								chNext = '\\';
								--p;
							}
						}
						break;

					default:
						chNext = '\\';
						--p;
						break;
					}

					start = p;
					stop = p;
					builder.Append(chNext);
				}
			}
			return null;
		}

		private static bool IsNumChar(char ch) {
			return (ch >= '0' && ch <= '9')
				|| (ch >= 'a' && ch <= 'z')
				|| (ch >= 'A' && ch <= 'Z')
				|| ch == '+' || ch == '-' || ch == '.';
		}

		private static int GetHexDigit(char ch) {
			if (ch >= '0' && ch <= '9') {
				return ch - '0';
			}
			if (ch >= 'a' && ch <= 'f') {
				return ch - 'a' + 10;
			}
			if (ch >= 'A' && ch <= 'F') {
				return ch - 'A' + 10;
			}
			return -1;
		}
	}


	internal enum JsonValueType {
		Null,
		Number,
		String,
		Array,
		Object,
	}

	internal class JsonValue {
		public readonly JsonValueType ValueType;
		private readonly string str;
		private readonly List<JsonValue> list;
		private readonly Dictionary<string, JsonValue> dict;

		public JsonValue(string value, JsonValueType type = JsonValueType.String) {
			ValueType = type;
			str = value;
		}

		public JsonValue(List<JsonValue> value) {
			ValueType = JsonValueType.Array;
			list = value;
		}

		public JsonValue(Dictionary<string, JsonValue> value) {
			ValueType = JsonValueType.Object;
			dict = value;
		}

		public int Count {
			get {
				if (ValueType == JsonValueType.Array) {
					return list.Count;
				}
				if (ValueType == JsonValueType.Object) {
					return dict.Count;
				}
				return 0;
			}
		}

		public JsonValue this[int index] {
			get { return list[index]; }
		}

		public JsonValue this[string key] {
			get { return dict[key]; }
		}

		public string GetString() {
			return str;
		}

		public int GetInt32() {
			return int.Parse(str, NumberFormatInfo.InvariantInfo);
		}

		public bool TryGetValue(string key, out JsonValue value) {
			return dict.TryGetValue(key, out value);
		}
	}
}
