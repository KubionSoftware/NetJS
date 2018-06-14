using Util;

namespace NetJS.Core.API {
    public class JSONAPI {

        public static Constant parse(Constant _this, Constant[] arguments, Agent agent) {
            var s = ((String)arguments[0]).Value.Trim();

            if(s.Length == 0) {
                return Static.Undefined;
            }

            if(s[0] == '{') {
                var json = JsonParser.StringToJsonObject(s);
                return Convert.JsonToObject(json, agent);
            } else if(s[0] == '[') {
                var json = JsonParser.StringToJsonList(s);
                return Convert.JsonToValue(json, agent);
            }

            return Static.Undefined;
        }

        public static Constant stringify(Constant _this, Constant[] arguments, Agent agent) {
            if (arguments.Length == 0) return Static.Undefined;

            var beautify = arguments.Length > 1 ? Tool.GetArgument<Boolean>(arguments, 1, "JSON.stringify").Value : false;

            var json = Convert.ValueToJson(arguments[0]);
            return new String(JsonParser.ValueToString(json, beautify));
        }
    }
}