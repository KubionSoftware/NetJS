using Util;

namespace NetJS.Internal {
    class JSON {

        public static Javascript.Constant parse(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var s = ((Javascript.String)arguments[0]).Value.Trim();

            if(s.Length == 0) {
                return Javascript.Static.Undefined;
            }

            if(s[0] == '{') {
                var json = JsonParser.StringToJsonObject(s);
                return Convert.JsonToObject(json, scope);
            } else if(s[0] == '[') {
                var json = JsonParser.StringToJsonList(s);
                return Convert.JsonToValue(json, scope);
            }

            return Javascript.Static.Undefined;
        }

        public static Javascript.Constant stringify(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            if (arguments.Length == 0) return Javascript.Static.Undefined;

            var beautify = arguments.Length > 1 ? Tool.GetArgument<Javascript.Boolean>(arguments, 1, "JSON.stringify").Value : false;

            var json = Convert.ValueToJson(arguments[0], scope);
            return new Javascript.String(JsonParser.ValueToString(json, beautify));
        }
    }
}