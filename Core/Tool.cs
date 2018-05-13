using System;
using System.Collections.Generic;

namespace NetJS.Core {
    public class Tool {

        public static string ToString(Javascript.Constant constant, Javascript.Scope scope) {
            if(constant == null || constant.IsUndefined()) {
                return "";
            }

            try {
                var node = new Javascript.Call() {
                    Left = new Javascript.Access(true) {
                        Left = constant,
                        Right = new Javascript.Variable("toString")
                    },
                    Right = new Javascript.ArgumentList()
                };

                var result = node.Execute(scope);

                if (result is Javascript.String s) {
                    return s.Value;
                }
            } catch {
                return constant.ToString();
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
            Javascript.Object obj = null;
            try {
                obj = scope.Engine.GetPrototype(name);
            } catch {
                if(scope.GetVariable(name) is Javascript.Object ob) {
                    obj = ob;
                } else {
                    throw new Javascript.InternalError($"Could not get prototype of '{name}'");
                }
            }
            
            var prototype = obj.Get("prototype");
            if (prototype is Javascript.Object o) {
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

            if (insidePath.Length == 0) return path;

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
            if (!(argument is T)) throw new Javascript.InternalError($"{context}: Expected argument with type '{typeof(T)}' at index {index}");

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
    }
}