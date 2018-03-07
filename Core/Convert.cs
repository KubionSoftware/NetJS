using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;

namespace NetJS.Core {
    public static class Convert {

        public static Constant JsonToValue(object value, Scope scope) {
            if (value is Dictionary<string, object> dict) {
                return JsonToObject(dict, scope);
            } else if (value is List<object> list) {
                var array = new Javascript.Array();
                
                for (int i = 0; i < list.Count; i++) {
                    array.List.Add(JsonToValue(list[i], scope));
                }

                return array;
            } else if (value is string s) {
                return new Javascript.String(s);
            } else if (value is double d) {
                return new Javascript.Number(d);
            } else if (value is long l) {
                return new Javascript.Number(l);
            } else if (value is int i) {
                return new Javascript.Number(i);
            } else if (value is bool b) {
                return new Javascript.Boolean(b);
            }

            return Static.Undefined;
        }

        public static Constant JsonToObject(Dictionary<string, object> json, Scope scope) {
            var obj = new Javascript.Object(Tool.Construct("Object", scope));
            foreach (var key in json.Keys) {
                obj.Set(key, JsonToValue(json[key], scope));
            }
            return obj;
        }

        public static object ValueToJson(Constant value, Scope scope) {
            if (value is Javascript.String s) {
                return s.Value;
            } else if (value is Javascript.Number n) {
                return n.Value;
            } else if (value is Javascript.Boolean b) {
                return b.Value;
            } else if (value is Javascript.Function) {
                return "f () {}";
            } else if (value is Javascript.Array array) {
                var list = new List<object>();
                for (var i = 0; i < array.List.Count; i++) {
                    list.Add(ValueToJson(array.List[i], scope));
                }
                return list;
            } else if (value is Javascript.Object obj) {
                return ObjectToJson(obj, scope);
            }

            return null;
        }

        public static Dictionary<string, object> ObjectToJson(Javascript.Object obj, Scope scope) {
            var json = new Dictionary<string, object>();
            foreach (var key in obj.GetKeys()) {
                json[key] = ValueToJson(obj.Get(key), scope);
            }
            return json;
        }

        public static DateTime UnixMillisecondsToDateTime(double milliseconds) {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return epoch.AddMilliseconds(milliseconds);
        }

        public static double DateTimeToUnixMilliseconds(DateTime date) {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (date.ToLocalTime() - epoch);
            return Math.Round(span.TotalMilliseconds);
        }
    }
}