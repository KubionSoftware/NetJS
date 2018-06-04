using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;

namespace NetJS.Core {
    public static class Convert {

        public static Constant JsonToValue(object value, Agent agent) {
            if (value is Dictionary<string, object> dict) {
                return JsonToObject(dict, agent);
            } else if (value is List<object> list) {
                var array = new Javascript.Array();
                
                for (int i = 0; i < list.Count; i++) {
                    array.List.Add(JsonToValue(list[i], agent));
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

        public static Constant JsonToObject(Dictionary<string, object> json, Agent agent) {
            var obj = new Javascript.Object(Tool.Construct("Object", agent));
            foreach (var key in json.Keys) {
                obj.Set(new Javascript.String(key), JsonToValue(json[key], agent));
            }
            return obj;
        }

        public static object ValueToJson(Constant value) {
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
                    list.Add(ValueToJson(array.List[i]));
                }
                return list;
            } else if (value is Javascript.Object obj) {
                return ObjectToJson(obj);
            }

            return null;
        }

        public static Dictionary<string, object> ObjectToJson(Javascript.Object obj) {
            var json = new Dictionary<string, object>();
            foreach (var key in obj.OwnPropertyKeys()) {
                json[key.ToString()] = ValueToJson(obj.Get(key));
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

        public static string ToString(Constant value, Agent agent) {
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
                    var method = o.Get(new Javascript.String("toString"));
                    if (method is Function f) {
                        var result = f.Call(o, agent);

                        if (result is Javascript.String rs) {
                            return rs.Value;
                        }
                    }
                    break;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to string");
        }

        public static Constant ToPrimitive(Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toprimitive

            if (value is Javascript.Object o) {
                var valueOf = o.Get(new Javascript.String("valueOf"));
                if (valueOf is Function vf) {
                    var result = vf.Call(o, agent);
                    if (!(result is Javascript.Object)) return result;
                }

                var toString = o.Get(new Javascript.String("toString"));
                if (toString is Function sf) {
                    var result = sf.Call(o, agent);
                    if (!(result is Javascript.Object)) return result;
                }

                throw new TypeError("Could not convert object to primitive");
            }

            return value;
        }

        public static double ToNumber(Constant value, Agent agent) {
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
                    var primitive = ToPrimitive(o, agent);
                    return ToNumber(primitive, agent);
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

        public static Javascript.Object ToObject(Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toobject

            switch (value) {
                case Javascript.Undefined u:
                    break;
                case Javascript.Null nu:
                    break;
                case Javascript.Boolean b:
                    return Tool.Construct("Boolean", agent, new[] { b });
                case Javascript.Number n:
                    return Tool.Construct("Number", agent, new[] { n });
                case Javascript.String s:
                    return Tool.Construct("String", agent, new[] { s });
                case Javascript.Symbol sy:
                    return Tool.Construct("Symbol", agent, new[] { sy });
                case Javascript.Object o:
                    return o;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to object");
        }

        public static Constant ToPropertyKey(Constant argument, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-topropertykey

            var key = ToPrimitive(argument, agent);

            if (key is Symbol) return key;

            return new Javascript.String(ToString(key, agent));
        }

        public static int ToInt32(Constant argument, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toint32

            var number = ToNumber(argument, agent);
            if (double.IsNaN(number) || double.IsInfinity(number)) return 0;

            var i = (int)Math.Floor(Math.Abs(number));
            var int32bit = i % (int)Math.Pow(2, 32);
            if (int32bit > Math.Pow(2, 31)) {
                return int32bit - (int)Math.Pow(2, 32);
            } else {
                return int32bit;
            }
        }

        public static int ToUint32(Constant argument, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-touint32

            var number = ToNumber(argument, agent);
            if (double.IsNaN(number) || double.IsInfinity(number)) return 0;

            var i = (int)Math.Floor(Math.Abs(number));
            var int32bit = i % (int)Math.Pow(2, 32);
            return int32bit;
        }
    }
}