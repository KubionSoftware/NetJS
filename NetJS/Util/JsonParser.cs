using System;
using System.Collections.Generic;
using System.Text;

namespace Util {
    
    public static class JsonParser {

        private const char ObjectStart = '{';
        private const char ObjectEnd = '}';
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';
        private const char StringStart = '"';
        private const char StringEnd = '"';
        private const char Delimiter = ',';
        private const char NewLine = '\n';
        private const char KeyValue = ':';

        private const int MaxDepth = 100;

        private static string Escape(string s) {
            var output = new StringBuilder();

            foreach(var c in s) {
                switch (c) {
                    case '\\': output.Append("\\\\"); break;
                    case '"': output.Append("\\\""); break;
                    case '\b': output.Append("\\b"); break;
                    case '\f': output.Append("\\f"); break;
                    case '\n': output.Append("\\n"); break;
                    case '\r': output.Append("\\r"); break;
                    case '\t': output.Append("\\t"); break;
                    default: output.Append(c); break;
                }
            }

            return output.ToString();
        }

        private static bool IsWhitespace(char c) {
            return c == ' ' || c == '\n' || c == '\t' || c == '\r';
        }

        private static bool IsNumber(char c) {
            return c == '.' || (c >= '0' && c <= '9') || c == 'e' || c == 'E' || c == '+' || c == '-';
        }

        private static void AppendTabs(StringBuilder builder, int num) {
            for(int i = 0; i < num; i++) {
                builder.Append('\t');
            }
        }

        private static void ValueToString(object value, StringBuilder builder, bool pretty, int level) {
            if (value == null) {
                builder.Append("null");
                return;
            }

            Type valueType = value.GetType();

            if (valueType == typeof(Dictionary<string, object>)) {
                JsonToString((Dictionary<string, object>)value, builder, pretty, level);
            } else if (valueType == typeof(List<object>)) {
                builder.Append(ArrayStart);
                level++;
                if (pretty) {
                    builder.Append(NewLine);
                    AppendTabs(builder, level);
                }

                List<object> array = (List<object>)value;
                for (int i = 0; i < array.Count; i++) {
                    ValueToString(array[i], builder, pretty, level);

                    if (i < array.Count - 1) {
                        builder.Append(Delimiter);
                        if (pretty) {
                            builder.Append(NewLine);
                            AppendTabs(builder, level);
                        }
                    }
                }

                if (pretty) {
                    builder.Append(NewLine);
                    level--;
                    AppendTabs(builder, level);
                }
                builder.Append(ArrayEnd);
            } else if (valueType == typeof(string)) {
                builder.Append(StringStart + Escape((string)value) + StringEnd);
            } else if (valueType == typeof(double)) {
                builder.Append(((double)value).ToString());
            } else if (valueType == typeof(bool)) {
                builder.Append((bool)value ? "true" : "false");
            }
        }

        public static string ValueToString(object value, bool pretty = false) {
            StringBuilder builder = new StringBuilder();
            ValueToString(value, builder, pretty, 0);
            return builder.ToString();
        }

        private static void JsonToString(Dictionary<string, object> json, StringBuilder builder, bool pretty, int level) {
            builder.Append(ObjectStart);
            level++;
            if (pretty) {
                builder.Append(NewLine);
                AppendTabs(builder, level);
            }

            bool first = true;

            foreach (string key in json.Keys) {
                if (!first) {
                    builder.Append(Delimiter);
                    if (pretty) {
                        builder.Append(NewLine);
                        AppendTabs(builder, level);
                    }
                }
                first = false;

                object value = json[key];

                builder.Append(StringStart + key + StringEnd + KeyValue);
                ValueToString(value, builder, pretty, level);
            }

            if (pretty) {
                builder.Append(NewLine);
                level--;
                AppendTabs(builder, level);
            }
            builder.Append(ObjectEnd);
        }

        public static string JsonToString(Dictionary<string, object> json, bool pretty = false) {
            StringBuilder builder = new StringBuilder();
            JsonToString(json, builder, pretty, 0);
            return builder.ToString();
        }

        public static object StringToJson(string json) {
            int index = 0;

            // Skip starting whitespace
            while(index < json.Length) {
                if (!IsWhitespace(json[index])) break;
                index++;
            }

            object value = ParseValue(json, ref index, 0);

            // Check if it really ended
            index++;
            while(index < json.Length) {
                if (!IsWhitespace(json[index])) throw new Exception($"'{json[index]}' found after json ended");
                index++;
            }

            return value;
        }

        public static Dictionary<string, object> StringToJsonObject(string json) {
            return (Dictionary<string, object>)StringToJson(json);
        }

        public static IList<object> StringToJsonList(string json) {
            return (IList<object>)StringToJson(json);
        }

        private static Dictionary<string, object> ParseObject(string json, ref int index, int depth) {
            index++;    // skip {

            Dictionary<string, object> obj = new Dictionary<string, object>();

            string key = null;
            bool keyValueDelimiter = false;
            bool delimiter = true;

            while(index < json.Length) {
                char c = json[index];

                if (c == ObjectEnd) {
                    if (delimiter && obj.Count > 0) throw new Exception("Object ended with comma");
                    if (keyValueDelimiter) throw new Exception("Object ended with colon");
                    if (key != null) throw new Exception("Object ended with key without value");

                    return obj;
                } else if (IsWhitespace(c)) {

                } else if (c == KeyValue) {
                    if (key == null) throw new Exception("Colon found before key");
                    if (keyValueDelimiter) throw new Exception("Two colons found next to each other in object");

                    keyValueDelimiter = true;
                } else if (c == Delimiter) {
                    if (key != null) throw new Exception("Object value can't start with a comma");
                    if (delimiter) throw new Exception("Two comma's found next to each other in object");

                    delimiter = true;
                } else if (key == null) {
                    if (c != StringStart) throw new Exception($"Object keys can't start with '{c}'");
                    if (!delimiter) throw new Exception("Object keys can't start before a comma seperator");

                    key = ParseString(json, ref index);
                } else if (key != null) {
                    if (!keyValueDelimiter) throw new Exception("Object values can't start before a colon seperator");
                    if (!delimiter) throw new Exception("Object values can't start before a comma seperator");

                    obj[key] = ParseValue(json, ref index, depth);
                    key = null;
                    keyValueDelimiter = false;
                    delimiter = false;
                }

                index++;
            }

            throw new Exception("Object was not closed");
        }

        private static object ParseValue(string json, ref int index, int depth) {
            if (depth > MaxDepth) throw new Exception($"Exeeded maximum depth of {MaxDepth}");

            char c = json[index];

            if(c == StringStart) {
                return ParseString(json, ref index);
            }else if(c == ArrayStart) {
                return ParseArray(json, ref index, depth + 1);
            }else if(c == ObjectStart) {
                return ParseObject(json, ref index, depth + 1);
            }else if(IsNumber(c)) {
                return ParseNumber(json, ref index);
            }else if(c == 't' || c == 'f') {
                return ParseBool(json, ref index);
            }else if(c == 'n') {
                return ParseNull(json, ref index);
            }

            throw new Exception("Invalid value");
        }

        private static IList<object> ParseArray(string json, ref int index, int depth) {
            index++;    // skip [

            IList<object> list = new List<object>();
            bool delimiter = true;

            while(index < json.Length) {
                char c = json[index];

                if(c == ArrayEnd) {
                    if (delimiter && list.Count > 0) throw new Exception("Array ended with comma seperator");
                    return list;
                } else if(c == Delimiter) {
                    if (delimiter) throw new Exception("Comma seperator found in array when no item preceeded it");
                    delimiter = true;
                } else if(!IsWhitespace(c)){
                    if (!delimiter) throw new Exception("Items in array were not seperated by a comma");
                    list.Add(ParseValue(json, ref index, depth));
                    delimiter = false;
                }

                index++;
            }

            throw new Exception("Array was not closed");
        }

        private static double ParseNumber(string json, ref int index) {
            string temp = "";

            while(index < json.Length) {
                char c = json[index];

                if(IsNumber(c)) {
                    temp += c;
                } else {
                    index--;
                    break;
                }

                index++;
            }

            double value;
            if(!double.TryParse(temp, out value)) {
                throw new Exception("Invalid double value");
            }

            return value;
        }

        private static string ParseString(string json, ref int index) {
            index++;    // skip "

            StringBuilder value = new StringBuilder();

            while (index < json.Length) {
                char c = json[index];

                if (c == StringEnd) {
                    return value.ToString();
                } else if (c == '\t') {
                    throw new Exception("Unescaped tab in string");
                } else if (c == '\n') {
                    throw new Exception("Unescaped newline in string");
                } else if (c == '\0') {
                    throw new Exception("Unescaped null character in string");
                } else if (c == '\r') {
                    throw new Exception("Unescaped cariage return in string");
                } else if (c == '\\') {
                    index++;
                    if (index >= json.Length) throw new Exception("Escape character '\' at end of json");

                    var next = json[index];
                    switch (next) {
                        case '\\': value.Append("\\"); break;
                        case '"': value.Append("\""); break;
                        case 'b': value.Append("\b"); break;
                        case 'f': value.Append("\f"); break;
                        case 'n': value.Append("\n"); break;
                        case 'r': value.Append("\r"); break;
                        case 't': value.Append("\t"); break;
                        default: throw new Exception("Cannot escape '" + next + "'");
                    }
                } else {
                    value.Append(c);
                }
                
                index++;
            }

            throw new Exception("String was never closed");
        }

        private static bool ParseBool(string json, ref int index) {
            if(json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e') {
                index += 3;
                return true;
            }else if(json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e') {
                index += 4;
                return false;
            }

            throw new Exception("Invalid boolean value");
        }

        private static object ParseNull(string json, ref int index) {
            if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l') {
                index += 3;
                return null;
            }

            throw new Exception("Invalid null value");
        }
    }
}