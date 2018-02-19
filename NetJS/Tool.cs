using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS {
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
            var obj = scope.Application.Global.GetPrototype(name);
            return (Javascript.Object)obj.Get("prototype");
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
                    insidePath += System.IO.Path.DirectorySeparatorChar + parts[i];
                } else if(parts[i] == "src") {
                    insidePath += parts[i];
                }
            }

            return insidePath;
        }

        public static T GetArgument<T>(Javascript.Constant[] arguments, int index, string context, bool required = true) where T : Javascript.Constant {
            if (index >= arguments.Length) {
                if (required) {
                    throw new Exception($"{context}: Expected argument with type '{typeof(T)}' at index {index}");
                } else {
                    return null;
                }
            }

            var argument = arguments[index];
            if (argument.GetType() != typeof(T)) throw new Exception($"{context}: Expected argument with type '{typeof(T)}' at index {index}");

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

        public static string GetBaseUrl(HttpRequest request) {
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/") {
                appUrl = "/" + appUrl;
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public static string[] GetPath(HttpRequest request) {
            var baseUrl = GetBaseUrl(request);
            var url = request.Url.ToString();

            var queryIndex = url.IndexOf("?");
            if (queryIndex != -1) {
                url = url.Substring(0, queryIndex);
            }

            if (url.StartsWith(baseUrl)) {
                url = url.Remove(0, baseUrl.Length);
            }

            var path = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return path;
        }

        public static Javascript.Object CreateRequest(HttpContext context, string[] path, Javascript.Scope scope) {
            var request = Tool.Construct("Object", scope);
            request.Set("path", Tool.ToArray(path, scope));

            var form = Tool.Construct("Object", scope);
            for (var i = 0; i < context.Request.Form.Count; i++) {
                var key = context.Request.Form.GetKey(i);
                var value = context.Request.Form.Get(i);
                form.Set(key, new Javascript.String(value));
            }
            request.Set("form", form);

            var parameters = Tool.Construct("Object", scope);
            var query = HttpUtility.ParseQueryString(context.Request.Url.Query);
            for (var i = 0; i < query.Count; i++) {
                var key = query.GetKey(i);
                if (key == null) continue;

                var value = query.Get(i);
                parameters.Set(key, new Javascript.String(value));
            }
            request.Set("params", parameters);

            var content = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            request.Set("content", new Javascript.String(content));

            request.Set("method", new Javascript.String(context.Request.HttpMethod));
            request.Set("url", new Javascript.String(context.Request.RawUrl));

            return request;
        }
    }
}