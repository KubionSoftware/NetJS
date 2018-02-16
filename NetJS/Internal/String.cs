using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetJS.Javascript;

namespace NetJS.Internal {
    class String {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Javascript.Object)_this;

            // TODO: set value

            return Static.Undefined;
        }

        public static Constant toString(Constant _this, Constant[] arguments, Scope scope) {
            return _this;
        }

        public static Constant charAt(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var index = (Javascript.Number)arguments[0];

            if(index.Value >= 0 && index.Value < str.Value.Length) {
                return new Javascript.String(str.Value[(int)index.Value].ToString());
            } else {
                return new Javascript.String("");
            }
        }

        public static Constant charCodeAt(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var index = (Javascript.Number)arguments[0];

            if (index.Value >= 0 && index.Value < str.Value.Length) {
                return new Javascript.Number(str.Value[(int)index.Value]);
            } else {
                return new Javascript.NaN();
            }
        }

        public static Constant startsWith(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var search = (Javascript.String)arguments[0];

            return new Javascript.Boolean(str.Value.StartsWith(search.Value));
        }

        public static Constant endsWith(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var search = (Javascript.String)arguments[0];

            return new Javascript.Boolean(str.Value.EndsWith(search.Value));
        }

        public static Constant includes(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var search = (Javascript.String)arguments[0];

            return new Javascript.Boolean(str.Value.Contains(search.Value));
        }

        public static Constant indexOf(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var search = (Javascript.String)arguments[0];

            return new Javascript.Number(str.Value.IndexOf(search.Value));
        }

        public static Constant toLowerCase(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            return new Javascript.String(str.Value.ToLower());
        }

        public static Constant toUpperCase(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            return new Javascript.String(str.Value.ToUpper());
        }

        public static Constant trim(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            return new Javascript.String(str.Value.Trim());
        }

        public static Constant substr(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var start = (int)((Javascript.Number)arguments[0]).Value;
            if (start < 0) start = str.Value.Length + start;

            var length = arguments.Length == 2 ? (int)arguments[1].As<Javascript.Number>().Value : (int)str.Value.Length - start;
            if (start + length > str.Value.Length) length = str.Value.Length - start;

            return new Javascript.String(str.Value.Substring(start, length));
        }

        public static Constant substring(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;

            var start = (int)((Javascript.Number)arguments[0]).Value;
            var end = arguments.Length == 2 ? (int)arguments[1].As<Javascript.Number>().Value : (int)str.Value.Length;

            if (start < 0) start = 0;
            if (end < 0) end = 0;

            if (start > str.Value.Length) start = str.Value.Length;
            if (end > str.Value.Length) end = str.Value.Length;

            if(start > end) {
                var temp = end;
                end = start;
                start = temp;
            }

            return new Javascript.String(str.Value.Substring(start, end - start));
        }

        public static Constant split(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;

            if(arguments.Length == 0) {
                return new Javascript.String(str.Value);
            } else {
                if (arguments[0] is Javascript.String) {
                    var separator = (Javascript.String)arguments[0];

                    if (separator.Value.Length == 0) {
                        // TODO: performance
                        return Tool.ToArray(str.Value.ToCharArray().Select(c => c.ToString()).ToArray(), scope);
                    } else {
                        return Tool.ToArray(str.Value.Split(new string[] { separator.Value }, StringSplitOptions.None), scope);
                    }
                }else if(arguments[0] is Javascript.Object) {
                    return RegExp.split(arguments[0], new Constant[] { _this }, scope);
                }

                throw new Exception("first argument of string.split must be a string or a RegExp");
            }
        }

        public static Constant replace(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;

            if(arguments[0] is Javascript.String) {
                var search = (Javascript.String)arguments[0];
                var newValue = (Javascript.String)arguments[1];

                return new Javascript.String(str.Value.Replace(search.Value, newValue.Value));
            } else if(arguments[0] is Javascript.Object) {
                return RegExp.replace(arguments[0], new Constant[] { str, arguments[1] }, scope);
            }

            throw new Exception("first argument of string.replace must be a string or a RegExp");
        }

        public static Constant match(Constant _this, Constant[] arguments, Scope scope) {
            return RegExp.match(arguments[0], new Constant[] { _this }, scope);
        }

        public static Constant search(Constant _this, Constant[] arguments, Scope scope) {
            return RegExp.search(arguments[0], new Constant[] { _this }, scope);
        }

        public static Constant test(Constant _this, Constant[] arguments, Scope scope) {
            return RegExp.test(arguments[0], new Constant[] { _this }, scope);
        }

        public static Constant repeat(Constant _this, Constant[] arguments, Scope scope) {
            var str = (Javascript.String)_this;
            var number = (Javascript.Number)arguments[0];

            var result = new StringBuilder();
            for(var i = 0; i < number.Value; i++) {
                result.Append(str.Value);
            }

            return new Javascript.String(result.ToString());
        }
    }
}
