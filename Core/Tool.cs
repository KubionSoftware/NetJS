using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core {
    public class Tool {

        public static string GetResultString(Constant result, Agent agent) {
            if (!(result is Core.Undefined)) {
                return Core.Convert.ToString(result, agent);
            } else {
                return agent.Running.Buffer.ToString();
            }
        }

        public static Array ToArray(IEnumerable<string> strings, Agent agent) {
            return ToArray(strings.Select(s => new String(s)), agent);
        }

        public static Array ToArray(IEnumerable<Constant> values, Agent agent) {
            var array = new Array(0, agent);
            array.AddRange(values, agent);
            return array;
        }

        public static Object Construct(string name, Agent agent, Expression[] arguments = null) {
            var objRef = new New() {
                NewExpression = new Identifier(name),
                Arguments = arguments != null ? new ArgumentList(arguments) : new ArgumentList()
            }.Evaluate(agent);
            var objVal = References.GetValue(objRef, agent);
            return (Object)objVal;
        }

        public static Object Prototype(string name, Agent agent) {
            Object obj = null;
            try {
                obj = agent.Running.Realm.GetPrototype(name);
            } catch {
                if(References.GetValue(new Identifier(name).Evaluate(agent), agent) is Object ob) {
                    obj = ob;
                } else {
                    throw new TypeError($"Could not get prototype of '{name}'");
                }
            }
            
            var prototype = obj.Get(new String("prototype"), agent);
            if (prototype is Object o) {
                return o;
            }

            throw new TypeError($"Could not get prototype of '{name}'");
        }

        public static bool IsType(Constant o, Constant c, Agent agent) {
            if (o is Object oo) {
                if (c is Object co) {
                    var p = co.Get(new String("prototype"), agent);
                    if (p is Object po) {
                        while (true) {
                            oo = oo.GetPrototypeOf();
                            if (oo == null) return false;
                            if (Compare.SameValue(po, oo)) return true;
                        }
                    } else {
                        throw new TypeError("Prototype is not an object");
                    }
                } else {
                    throw new TypeError("Right-hand side of instanceof operator must be an object");
                }
            } else {
                return false;
            }
        }

        public static string NormalizePath(string path) {
            path = System.IO.Path.GetFullPath(path)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                .ToLowerInvariant();

            var parts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            var insidePath = "";

            for (var i = 0; i < parts.Length; i++) {
                if(insidePath.Length > 0) {
                    insidePath += "/" + parts[i];
                } else if(parts[i] == "src") {
                    insidePath = parts[i];
                }
            }

            if (insidePath.Length == 0) return path;

            return insidePath;
        }

        public static T GetArgument<T>(Constant[] arguments, int index, string context, bool required = true) where T: Constant {
            if (index >= arguments.Length) {
                if (required) {
                    throw new TypeError($"{context}: Expected argument with type '{typeof(T)}' at index {index}");
                } else {
                    return default(T);
                }
            }

            var argument = arguments[index];
            if (!(argument is T)) throw new TypeError($"{context}: Expected argument with type '{typeof(T)}' at index {index}");

            return (T)argument;
        }

        public static Constant GetArgument(Constant[] arguments, int index, string context, bool required = true) {
            if (index >= arguments.Length) {
                if (required) {
                    throw new TypeError($"{context}: Expected argument at index {index}");
                } else {
                    return null;
                }
            }

            return arguments[index];
        }

        public static bool GetDate(Constant c, out DateTime date, Agent agent) {
            if (c is Object obj && obj.Get(API.DateAPI.Primitive, agent) is Foreign f && f.Value is DateTime d) {
                date = d;
                return true;
            } else {
                date = DateTime.Now;
                return false;
            }
        }
    }
}