using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core {
    public static class Convert {

        public static Constant JsonToValue(object value, Agent agent) {
            if (value is Dictionary<string, object> dict) {
                return JsonToObject(dict, agent);
            } else if (value is List<object> list) {
                var array = new Array(0, agent);
                array.AddRange(list.Select(item => JsonToValue(item, agent)));
                return array;
            } else if (value is string s) {
                return new String(s);
            } else if (value is double d) {
                return new Number(d);
            } else if (value is long l) {
                return new Number(l);
            } else if (value is int i) {
                return new Number(i);
            } else if (value is bool b) {
                return Boolean.Create(b);
            }

            return Static.Undefined;
        }

        public static Constant JsonToObject(Dictionary<string, object> json, Agent agent) {
            var obj = new Object(Tool.Construct("Object", agent));
            foreach (var key in json.Keys) {
                obj.Set(new String(key), JsonToValue(json[key], agent));
            }
            return obj;
        }

        public static object ValueToJson(Constant value, Agent agent, List<Object> objects = null) {
            if (objects == null) objects = new List<Object>();

            if (value is String s) {
                return s.Value;
            } else if (value is Number n) {
                return n.Value;
            } else if (value is Boolean b) {
                return b.Value;
            } else if (value is Function) {
                return "f (){}";
            } else if (value is Array array) {
                if (objects.Contains(array)) throw new TypeError("Converting circular structure to JSON");
                objects.Add(array);

                var list = new List<object>();
                for (var i = 0; i < array.List.Count; i++) {
                    list.Add(ValueToJson(array.List[i], agent, objects));
                }
                return list;
            } else if (value is Object obj) {
                if (objects.Contains(obj)) throw new TypeError("Converting circular structure to JSON");
                objects.Add(obj);

                return ObjectToJson(obj, agent, objects);
            }

            return null;
        }

        private static Dictionary<string, object> ObjectToJson(Object obj, Agent agent, List<Object> objects) {
            var json = new Dictionary<string, object>();
            foreach (var key in obj.OwnPropertyKeys()) {
                json[key.ToString()] = ValueToJson(obj.Get(key, agent), agent, objects);
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
                case Undefined u:
                    return "undefined";
                case Null nu:
                    return "null";
                case Boolean b:
                    return b.Value ? "true" : "false";
                case Number n:
                    return n.Value.ToString();
                case String s:
                    return s.Value;
                case Symbol sy:
                    break;
                case Object o:
                    var method = o.Get(new String("toString"), agent);
                    if (method is Function f) {
                        var result = f.Call(o, agent);

                        if (result is String rs) {
                            return rs.Value;
                        }
                    }
                    break;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to string");
        }

        public static Constant ToPrimitive(Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toprimitive

            if (value is Object o) {
                var valueOf = o.Get(new String("valueOf"), agent);
                if (valueOf is Function vf) {
                    var result = vf.Call(o, agent);
                    if (!(result is Object)) return result;
                }

                var toString = o.Get(new String("toString"), agent);
                if (toString is Function sf) {
                    var result = sf.Call(o, agent);
                    if (!(result is Object)) return result;
                }

                throw new TypeError("Could not convert object to primitive");
            }

            return value;
        }

        public static double ToNumber(Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-tonumber

            switch (value) {
                case Undefined u:
                    return double.NaN;
                case Null nu:
                    return 0;
                case Boolean b:
                    return b.Value ? 1 : 0;
                case Number n:
                    return n.Value;
                case String s:
                    if(double.TryParse(s.Value, out double d)) {
                        return d;
                    } else {
                        return double.NaN;
                    }
                case Symbol sy:
                    break;
                case Object o:
                    var primitive = ToPrimitive(o, agent);
                    return ToNumber(primitive, agent);
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to number");
        }

        public static bool ToBoolean(Constant value) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toboolean

            switch (value) {
                case Undefined u:
                    return false;
                case Null nu:
                    return false;
                case Boolean b:
                    return b.Value;
                case Number n:
                    return n.Value == 0 ? false : true;
                case String s:
                    return s.Value.Length == 0 ? false : true;
                case Symbol sy:
                    return true;
                case Object o:
                    return true;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to boolean");
        }

        public static Object ToObject(Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-toobject

            switch (value) {
                case Undefined u:
                    break;
                case Null nu:
                    break;
                case Boolean b:
                    return Tool.Construct("Boolean", agent, new[] { new BooleanLiteral(b.Value) });
                case Number n:
                    return Tool.Construct("Number", agent, new[] { new NumberLiteral(n.Value) });
                case String s:
                    return Tool.Construct("String", agent, new[] { new StringLiteral(s.Value) });
                case Symbol sy:
                    return Tool.Construct("Symbol", agent, new[] { new StringLiteral("") });
                case Object o:
                    return o;
            }

            throw new TypeError($"Could not convert '{value.ToDebugString()}' to object");
        }

        public static Constant ToPropertyKey(Constant argument, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-topropertykey

            var key = ToPrimitive(argument, agent);

            if (key is Symbol) return key;

            return new String(ToString(key, agent));
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