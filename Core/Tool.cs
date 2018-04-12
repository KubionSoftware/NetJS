using System;
using System.Collections.Generic;

namespace NetJS.Core {
    public class Tool {

        public static string ToString(Javascript.Constant constant, Javascript.Scope scope) {
            var node = new Javascript.Call() {
                Left = new Javascript.Access(true) {
                    Left = constant,
                    Right = new Javascript.Variable("toString")
                },
                Right = new Javascript.ArgumentList() { Arguments = new Javascript.Constant[] { } }
            };

            var result = node.Execute(scope);

            if(result is Javascript.String s) {
                return s.Value;
            }

            return "";
        }

        public static Javascript.Array ToArray(IList<string> list, Javascript.Scope scope) {
            var array = new Javascript.Array();
            foreach(var s in list) {
                array.List.Add(new Javascript.String(s));
            }
            return array;
        }

        public static Javascript.Object Construct(string name, Javascript.Scope scope) {
            return new Javascript.Object(Prototype(name, scope));
        }

        public static Javascript.Object Prototype(string name, Javascript.Scope scope) {
            var obj = scope.Engine.GetPrototype(name);
            var prototype = obj.Get("prototype");
            if(prototype is Javascript.Object o) {
                return o;
            }
            throw new Javascript.InternalError($"Could not get prototype of '{name}'");
        }

        public static bool IsType(Javascript.Object obj, Javascript.Object prototype) {
            return obj.__proto__ == prototype.Get<Javascript.Object>("prototype");
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

            if (insidePath.Length == 0) throw new Javascript.InternalError("Could not find src folder");

            return insidePath;
        }

        public static T GetArgument<T>(Javascript.Constant[] arguments, int index, string context, bool required = true) where T : Javascript.Constant {
            if (index >= arguments.Length) {
                if (required) {
                    throw new Javascript.InternalError($"{context}: Expected argument with type '{typeof(T)}' at index {index}");
                } else {
                    return null;
                }
            }

            var argument = arguments[index];
            if (argument.GetType() != typeof(T)) throw new Javascript.InternalError($"{context}: Expected argument with type '{typeof(T)}' at index {index}");

            return (T)argument;
        }

        public static Javascript.Constant GetArgument(Javascript.Constant[] arguments, int index, string context, bool required = true) {
            if (index >= arguments.Length) {
                if (required) {
                    throw new Exception($"{context}: Expected argument at index {index}");
                } else {
                    return null;
                }
            }

            return arguments[index];
        }

        public static bool CheckType(Javascript.Constant value, string type) {
            if (type == "any") {
                return true;
            } else if (type == "string") {
                if (!(value is Javascript.String)) return false;
            } else if (type == "number") {
                if (!(value is Javascript.Number)) return false;
            } else if (type == "boolean") {
                if (!(value is Javascript.Boolean)) return false;
            } else if (type == "object") {
                if (!(value is Javascript.Object)) return false;
            } else if (type == "Date") {
                if (!(value is Javascript.Date)) return false;
            } else if (type.EndsWith("[]")) {
                var itemType = type.Replace("[]", "");
                if (!(value is Javascript.Array)) return false;

                var array = (Javascript.Array)value;
                for(var i = 0; i < array.List.Count; i++) {
                    if (!CheckType(array.List[i], itemType)) return false;
                }
            }

            return true;
        }
    }
}