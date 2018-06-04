using NetJS.Core.Javascript;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NetJS.Core.API {
    public class Functions {

        public static Constant parseInt(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            if (arguments[0] is Javascript.String s) {
                int intResult;
                if(int.TryParse(s.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intResult)) {
                    return new Javascript.Number(intResult);
                }

                double floatResult;
                if (double.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatResult)) {
                    return new Javascript.Number((int)floatResult);
                }
            } else if (arguments[0] is Javascript.Number n) {
                return new Javascript.Number((int)n.Value);
            }

            return Static.NaN;
        }

        public static Constant parseFloat(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            if (arguments[0] is Javascript.String s) {
                double result;
                if (double.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) {
                    return new Javascript.Number(result);
                }
            } else if (arguments[0] is Javascript.Number n) {
                return new Javascript.Number(n.Value);
            }

            return Static.NaN;
        }

        public static Constant encodeURI(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var uriString = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new Javascript.String(uri.GetLeftPart(UriPartial.Path) + uri.Query);
        }

        public static Constant decodeURI(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var uriString = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new Javascript.String(uri.GetLeftPart(UriPartial.Path) + Util.Encode.UrlDecode(uri.Query));
        }

        public static Constant encodeURIComponent(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var uri = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURIComponent");
            return new Javascript.String(new Regex(@"%[a-f0-9]{2}").Replace(Util.Encode.UrlEncode(uri.Value), m => m.Value.ToUpperInvariant()));
        }

        public static Constant decodeURIComponent(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var uri = Tool.GetArgument<Javascript.String>(arguments, 0, "decodeURIComponent");
            return new Javascript.String(Util.Encode.UrlDecode(uri.Value));
        }

        public static Constant eval(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var code = Tool.GetArgument<Javascript.String>(arguments, 0, "eval");

            try {
                // TODO: file id
                var tokens = new Lexer(code.Value, -1).Lex();
                var parser = new Parser(-1, tokens);
                var block = parser.Parse();

                return block.Execute(lex).Value;
            } catch { }

            return Javascript.Static.Undefined;
        }
    }
}