using System.Text;
using System.Web;
using NetJS.Core.Javascript;

namespace NetJS.External {
    /// <summary>Functions class contain functions that are injected directly into the engine.</summary>
    public class Functions {

        private static Constant includeLoad(Constant[] arguments, bool returnVar, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "include");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            // Create scope
            var buffer = returnVar ? new StringBuilder() : scope.Buffer;
            var templateScope = new Scope(application.Engine.Scope, scope, node, ScopeType.Template, buffer);

            // TODO: THIS IS A MEGA-HACK, REMOVE AS SOON AS POSSIBLE!!!
            foreach (var key in scope.Variables) {
                if (key.StartsWith("__") && key.EndsWith("__")) {
                    templateScope.SetVariable(key, scope.GetVariable(key));
                }
            }

            // Pass arguments
            if (arguments.Length > 1) {
                var parameters = (Object)arguments[1];
                foreach (var key in parameters.GetKeys()) {
                    templateScope.SetVariable(key, parameters.Get(key));
                }
            }

            // Execute template
            var result = node.Execute(templateScope).Constant;

            if (returnVar) {
                return result.IsUndefined() ? new String(buffer.ToString()) : result;
            } else {
                buffer.Append(result.ToString());
                return Static.Undefined;
            }
        }
        
        /// <summary>include  takes a file, runs the code in the file and writes the result to the output buffer.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to include</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">include("renderLayout.js", {loggedIn: true});</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant include(Constant _this, Constant[] arguments, Scope scope) {
            return includeLoad(arguments, false, scope);
        }

        /// <summary>load takes a file, runs the code in the file and returns the value.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to load</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">var output = load("renderLayout.js", {loggedIn: true});</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant load(Constant _this, Constant[] arguments, Scope scope) {
            return includeLoad(arguments, true, scope);
        }

        /// <summary>out writes a string to the output buffer
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">out(JSON.stringify(data));</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant @out(Constant _this, Constant[] arguments, Scope scope) {
            var value = Core.Tool.GetArgument(arguments, 0, "out");
            scope.Buffer.Append(Core.Tool.ToString(value, scope));
            return Static.Undefined;
        }

        /// <summary>outLine writes a string to the output buffer and appends a newline
        /// <param name="value">The string to write</param>
        /// <example><code lang="javascript">outLine(JSON.stringify(data));</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant @outLine(Constant _this, Constant[] arguments, Scope scope) {
            var value = Core.Tool.GetArgument(arguments, 0, "outLine");
            scope.Buffer.Append(Core.Tool.ToString(value, scope) + "\n");
            return Static.Undefined;
        }

        /// <summary>import takes a file and runs the code in the file with the current scope.
        /// This way functions and variables can be imported.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to import</param>
        /// <example><code lang="javascript">import("date");
        /// FormatDate(new Date());</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant import(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "import");

            var parts = name.Value.Split('.');
            if (parts.Length == 1) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            if (scope.StackParent == null) throw new InternalError("No scope to import code in");
            return node.Execute(scope.StackParent).Constant;
        }

        
        /// <summary>redirect takes an url and redirects a HttpResponse to the given url.</summary>
        /// <param name="url">A url to redirect to</param>
        /// <example><code lang="javascript">redirect("https://google.com/search?q=hello+world");</code></example>
        public static Constant redirect(Constant _this, Constant[] arguments, Scope scope) {
            var url = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "redirect");
            var context = HttpContext.Current;
            if (context != null) {
                context.Response.Redirect(url.Value);
            }
            return Static.Undefined;
        }
    }
}