using NetJS.Core;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NetJS.Core.API {
    public class FunctionsAPI {

        public static Constant parseInt(Constant _this, Constant[] arguments, Agent agent) {
            var value = Tool.GetArgument(arguments, 0, "parseInt");

            if (value is String s) {
                int intResult;
                if(int.TryParse(s.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intResult)) {
                    return new Number(intResult);
                }

                double floatResult;
                if (double.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatResult)) {
                    return new Number((int)floatResult);
                }
            } else if (value is Number n) {
                return new Number((int)n.Value);
            }

            return Static.NaN;
        }

        public static Constant parseFloat(Constant _this, Constant[] arguments, Agent agent) {
            var value = Tool.GetArgument(arguments, 0, "parseFloat");

            if (value is String s) {
                double result;
                if (double.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) {
                    return new Number(result);
                }
            } else if (value is Number n) {
                return new Number(n.Value);
            }

            return Static.NaN;
        }

        public static Constant isNaN(Constant _this, Constant[] arguments, Agent agent) {
            return Boolean.Create(Tool.GetArgument(arguments, 0, "isNaN") is NaN);
        }

        public static Constant encodeURI(Constant _this, Constant[] arguments, Agent agent) {
            var uriString = Tool.GetArgument<String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new String(uri.GetLeftPart(UriPartial.Path) + uri.Query);
        }

        public static Constant decodeURI(Constant _this, Constant[] arguments, Agent agent) {
            var uriString = Tool.GetArgument<String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new String(uri.GetLeftPart(UriPartial.Path) + Util.Encode.UrlDecode(uri.Query));
        }

        public static Constant encodeURIComponent(Constant _this, Constant[] arguments, Agent agent) {
            var uri = Tool.GetArgument<String>(arguments, 0, "encodeURIComponent");
            return new String(new Regex(@"%[a-f0-9]{2}").Replace(Util.Encode.UrlEncode(uri.Value), m => m.Value.ToUpperInvariant()));
        }

        public static Constant decodeURIComponent(Constant _this, Constant[] arguments, Agent agent) {
            var uri = Tool.GetArgument<String>(arguments, 0, "decodeURIComponent");
            return new String(Util.Encode.UrlDecode(uri.Value));
        }

        public static Constant eval(Constant _this, Constant[] arguments, Agent agent) {
            var code = Tool.GetArgument<String>(arguments, 0, "eval");

            try {
                // TODO: file id
                var tokens = new Lexer(code.Value, -1).Lex();
                var parser = new Parser(-1, tokens);
                var block = parser.Parse();

                return block.Evaluate(agent).Value;
            } catch { }

            return Static.Undefined;
        }
    }
}