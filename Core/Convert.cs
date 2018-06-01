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

        public static string ToString(Constant value, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-tostring

            switch (value) {
                case Javascript.Undefined u:
                    return "undefined";
                case Javascript.Null nu:
                    return "null";
                case Javascript.Boolean b:
                    return b.Value ? "true" : "false";
                case Javascript.Number n:
                    return n.Value.ToString();
                case Javascript.String s:
                    return s.Value;
                case Javascript.Symbol sy:
                    break;
                case Javascript.Object o:
                    var method = o.GetProperty(new Javascript.String("toString"), scope);
                    if (method is Function f) {
                        var result = f.Call(new Constant[] { }, o, scope);

                        if (result is Javascript.String rs) {
                            return rs.Value;
                        }
                    }
                    break;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to string");
        }

        public static Constant ToPrimitive(Constant value, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toprimitive

            if (value is Javascript.Object o) {
                var valueOf = o.GetProperty(new Javascript.String("valueOf"), scope);
                if (valueOf is Function vf) {
                    var result = vf.Call(new Constant[] { }, o, scope);
                    if (!(result is Javascript.Object)) return result;
                }

                var toString = o.GetProperty(new Javascript.String("toString"), scope);
                if (toString is Function sf) {
                    var result = sf.Call(new Constant[] { }, o, scope);
                    if (!(result is Javascript.Object)) return result;
                }

                throw new TypeError("Could not convert object to primitive");
            }

            return value;
        }

        public static double ToNumber(Constant value, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-tonumber

            switch (value) {
                case Javascript.Undefined u:
                    return double.NaN;
                case Javascript.Null nu:
                    return 0;
                case Javascript.Boolean b:
                    return b.Value ? 1 : 0;
                case Javascript.Number n:
                    return n.Value;
                case Javascript.String s:
                    if(double.TryParse(s.Value, out double d)) {
                        return d;
                    } else {
                        return double.NaN;
                    }
                case Javascript.Symbol sy:
                    break;
                case Javascript.Object o:
                    var primitive = ToPrimitive(o, scope);
                    return ToNumber(primitive, scope);
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to number");
        }

        public static bool ToBoolean(Constant value) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toboolean

            switch (value) {
                case Javascript.Undefined u:
                    return false;
                case Javascript.Null nu:
                    return false;
                case Javascript.Boolean b:
                    return b.Value;
                case Javascript.Number n:
                    return n.Value == 0 ? false : true;
                case Javascript.String s:
                    return s.Value.Length == 0 ? false : true;
                case Javascript.Symbol sy:
                    return true;
                case Javascript.Object o:
                    return true;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to boolean");
        }

        public static Javascript.Object ToObject(Constant value, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toobject

            switch (value) {
                case Javascript.Undefined u:
                    break;
                case Javascript.Null nu:
                    break;
                case Javascript.Boolean b:
                    return Tool.Construct("Boolean", scope, new[] { b });
                case Javascript.Number n:
                    return Tool.Construct("Number", scope, new[] { n });
                case Javascript.String s:
                    return Tool.Construct("String", scope, new[] { s });
                case Javascript.Symbol sy:
                    return Tool.Construct("Symbol", scope, new[] { sy });
                case Javascript.Object o:
                    return o;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to object");
        }

        public static Constant ToPropertyKey(Constant argument, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-topropertykey

            var key = ToPrimitive(argument, scope);

            if (key is Symbol) return key;

            return new Javascript.String(ToString(key, scope));
        }
    }
}