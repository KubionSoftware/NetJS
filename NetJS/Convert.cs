using NetJS.Javascript;
using System;
using System.Collections.Generic;

namespace NetJS {
    static class Convert {

        public static Constant JsonToValue(object value, Scope scope) {
            if (value is Dictionary<string, object>) {
                return JsonToObject((Dictionary<string, object>)value, scope);
            } else if (value is List<object>) {
                var array = new Javascript.Array();

                List<object> list = (List<object>)value;
                for (int i = 0; i < list.Count; i++) {
                    array.List.Add(JsonToValue(list[i], scope));
                }

                return array;
            } else if (value is string) {
                return new Javascript.String((string)value);
            } else if (value is double) {
                return new Javascript.Number((double)value);
            } else if (value is long) {
                return new Javascript.Number((long)value);
            } else if (value is int) {
                return new Javascript.Number((int)value);
            } else if (value is bool) {
                return new Javascript.Boolean((bool)value);
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
            if (value is Javascript.String) {
                return ((Javascript.String)value).Value;
            } else if (value is Javascript.Number) {
                return ((Javascript.Number)value).Value;
            } else if (value is Javascript.Boolean) {
                return ((Javascript.Boolean)value).Value;
            } else if (value is Javascript.Function) {
                return "f () {}";
            } else if (value is Javascript.Array) {
                var array = (Javascript.Array)value;
                var list = new List<object>();
                for (var i = 0; i < array.List.Count; i++) {
                    list.Add(ValueToJson(array.List[i], scope));
                }
                return list;
            } else if (value is Javascript.Object) {
                var obj = (Javascript.Object)value;
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