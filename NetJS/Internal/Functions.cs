using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NetJS.Internal {
    class Functions {

        public static Constant parseInt(Constant _this, Constant[] arguments, Scope scope) {
            if (arguments[0] is Javascript.String) {
                int intResult;
                if(int.TryParse(((Javascript.String)arguments[0]).Value, out intResult)) {
                    return new Javascript.Number(intResult);
                }

                double floatResult;
                if (double.TryParse(((Javascript.String)arguments[0]).Value, out floatResult)) {
                    return new Javascript.Number((int)floatResult);
                }
            } else if (arguments[0] is Javascript.Number) {
                return new Javascript.Number((int)((Javascript.Number)arguments[0]).Value);
            }

            return Static.NaN;
        }

        public static Constant parseFloat(Constant _this, Constant[] arguments, Scope scope) {
            if (arguments[0] is Javascript.String) {
                double result;
                if (double.TryParse(((Javascript.String)arguments[0]).Value, out result)) {
                    return new Javascript.Number(result);
                }
            } else if (arguments[0] is Javascript.Number) {
                return new Javascript.Number(((Javascript.Number)arguments[0]).Value);
            }

            return Static.NaN;
        }

        public static Constant encodeURI(Constant _this, Constant[] arguments, Scope scope) {
            var uriString = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new Javascript.String(uri.GetLeftPart(UriPartial.Path) + uri.Query);
        }

        public static Constant decodeURI(Constant _this, Constant[] arguments, Scope scope) {
            var uriString = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURI");
            var uri = new Uri(uriString.Value);
            return new Javascript.String(uri.GetLeftPart(UriPartial.Path) + HttpUtility.UrlDecode(uri.Query));
        }

        public static Constant encodeURIComponent(Constant _this, Constant[] arguments, Scope scope) {
            var uri = Tool.GetArgument<Javascript.String>(arguments, 0, "encodeURIComponent");
            return new Javascript.String(new Regex(@"%[a-f0-9]{2}").Replace(HttpUtility.UrlEncode(uri.Value), m => m.Value.ToUpperInvariant()));
        }

        public static Constant decodeURIComponent(Constant _this, Constant[] arguments, Scope scope) {
            var uri = Tool.GetArgument<Javascript.String>(arguments, 0, "decodeURIComponent");
            return new Javascript.String(HttpUtility.UrlDecode(uri.Value));
        }

        public static Constant eval(Constant _this, Constant[] arguments, Scope scope) {
            var code = Tool.GetArgument<Javascript.String>(arguments, 0, "eval");

            try {
                var tokens = Lexer.Lex(code.Value);
                var parser = new Parser("eval", tokens);
                var block = parser.Parse();

                return block.Execute(scope).Constant;
            } catch { }

            return Javascript.Static.Undefined;
        }

        public static Constant uneval(Constant _this, Constant[] arguments, Scope scope) {
            var builder = new StringBuilder();

            if (arguments.Length > 0) {
                arguments[0].Uneval(builder, 0);
            }

            return new Javascript.String(builder.ToString());
        }

        public static Constant include(Constant _this, Constant[] arguments, Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.include");
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var node = scope.Application.Global.GetFile(name.Value, scope.Application);
            var templateScope = new Scope(scope.Application.Global.Scope, node, scope.Session, scope.SVCache);

            if (arguments.Length > 1) {
                var parameters = (Javascript.Object)arguments[1];
                foreach (var key in parameters.GetKeys()) {
                    templateScope.SetVariable(key, parameters.Get(key));
                }
            }

            return node.Execute(templateScope).Constant;
        }

        public static Constant import(Constant _this, Constant[] arguments, Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.import");
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var node = scope.Application.Global.GetFile(name.Value, scope.Application);
            return node.Execute(scope).Constant;
        }
    }
}