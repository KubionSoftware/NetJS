using System;
using System.Linq;
using System.Text;
using NetJS.Core;

namespace NetJS.Core.API {
    class StringAPI {

        private const string Primitive = "[[PrimitiveValue]]";
        
        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var value = arguments.Length == 1 ? Tool.GetArgument<String>(arguments, 0, "String constructor") : new String("");
            var obj = _this as Object;
            obj.Set(Primitive, value);
            obj.Set("length", new Number(value.Value.Length));
            return _this;
        }

        private static String GetString(Constant _this, Agent agent) {
            if (_this is String s) return s;
            return (_this as Object).Get(Primitive, agent) as String;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Agent agent) {
            return GetString(_this, agent);
        }

        public static Constant charAt(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var index = (Number)arguments[0];

            if(index.Value >= 0 && index.Value < str.Value.Length) {
                return new String(str.Value[(int)index.Value].ToString());
            } else {
                return new String("");
            }
        }

        public static Constant charCodeAt(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var index = (Number)arguments[0];

            if (index.Value >= 0 && index.Value < str.Value.Length) {
                return new Number(str.Value[(int)index.Value]);
            } else {
                return new NaN();
            }
        }

        public static Constant startsWith(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var search = (String)arguments[0];

            return Boolean.Create(str.Value.StartsWith(search.Value));
        }

        public static Constant endsWith(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var search = (String)arguments[0];

            return Boolean.Create(str.Value.EndsWith(search.Value));
        }

        public static Constant includes(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var search = (String)arguments[0];

            return Boolean.Create(str.Value.Contains(search.Value));
        }

        public static Constant indexOf(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var search = (String)arguments[0];

            return new Number(str.Value.IndexOf(search.Value));
        }

        public static Constant toLowerCase(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            return new String(str.Value.ToLower());
        }

        public static Constant toUpperCase(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            return new String(str.Value.ToUpper());
        }

        public static Constant trim(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            return new String(str.Value.Trim());
        }

        public static Constant substr(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var start = (int)((Number)arguments[0]).Value;
            if (start < 0) start = str.Value.Length + start;

            var length = arguments.Length == 2 ? (int)Tool.GetArgument<Number>(arguments, 1, "String.substr").Value : (int)str.Value.Length - start;
            if (start + length > str.Value.Length) length = str.Value.Length - start;

            return new String(str.Value.Substring(start, length));
        }

        public static Constant substring(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);

            var start = (int)((Number)arguments[0]).Value;
            var end = arguments.Length == 2 ? (int)Tool.GetArgument<Number>(arguments, 1, "String.substring").Value : (int)str.Value.Length;

            if (start < 0) start = 0;
            if (end < 0) end = 0;

            if (start > str.Value.Length) start = str.Value.Length;
            if (end > str.Value.Length) end = str.Value.Length;

            if(start > end) {
                var temp = end;
                end = start;
                start = temp;
            }

            return new String(str.Value.Substring(start, end - start));
        }

        public static Constant split(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);

            if (arguments.Length == 0) {
                return new String(str.Value);
            } else {
                if (arguments[0] is String) {
                    var separator = (String)arguments[0];

                    if (separator.Value.Length == 0) {
                        return Tool.ToArray(str.Value.ToCharArray().Select(c => c.ToString()).ToArray(), agent);
                    } else {
                        return Tool.ToArray(str.Value.Split(new string[] { separator.Value }, StringSplitOptions.None), agent);
                    }
                }else if(arguments[0] is Object) {
                    return RegExpAPI.split(arguments[0], new Constant[] { GetString(_this, agent) }, agent);
                }

                throw new Exception("first argument of string.split must be a string or a RegExp");
            }
        }

        public static Constant replace(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);

            if (arguments[0] is String) {
                var search = (String)arguments[0];
                var newValue = (String)arguments[1];

                return new String(str.Value.Replace(search.Value, newValue.Value));
            } else if(arguments[0] is Object) {
                return RegExpAPI.replace(arguments[0], new Constant[] { str, arguments[1] }, agent);
            }

            throw new Exception("first argument of string.replace must be a string or a RegExp");
        }

        public static Constant match(Constant _this, Constant[] arguments, Agent agent) {
            return RegExpAPI.match(arguments[0], new Constant[] { GetString(_this, agent) }, agent);
        }

        public static Constant search(Constant _this, Constant[] arguments, Agent agent) {
            return RegExpAPI.search(arguments[0], new Constant[] { GetString(_this, agent) }, agent);
        }

        public static Constant test(Constant _this, Constant[] arguments, Agent agent) {
            return RegExpAPI.test(arguments[0], new Constant[] { GetString(_this, agent) }, agent);
        }

        public static Constant repeat(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var number = (Number)arguments[0];

            var result = new StringBuilder();
            for(var i = 0; i < number.Value; i++) {
                result.Append(str.Value);
            }

            return new String(result.ToString());
        }

        public static Constant getBytes(Constant _this, Constant[] arguments, Agent agent) {
            var str = GetString(_this, agent);
            var bytes = Encoding.Default.GetBytes(str.Value);

            return new NetJS.Core.Uint8Array(new ArrayBuffer(bytes), agent);
        }
    }
}
