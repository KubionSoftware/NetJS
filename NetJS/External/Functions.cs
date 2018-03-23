using System.Web;
using NetJS.Core.Javascript;

namespace NetJS.External {
    /// <summary>Functions class contain functions that are injected directly into the engine.</summary>
    public class Functions {
        
        /// <summary>include  takes a file, runs the code in the file and returns the value.
        /// If an object is given as second parameter, those variables will be set in the code before execution.
        /// Default filetype is ".js".</summary>
        /// <param name="file">The file to include</param>
        /// <param name="variables">An object with variables to setup the file before execution</param>
        /// <example><code lang="javascript">include("renderLayout.js", {loggedIn: true});</code></example>
        /// <exception cref="InternalError">Thrown when no application is found in application scope.</exception>
        public static Constant include(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "include");
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            var templateScope = new Scope(application.Engine.Scope, scope, node, ScopeType.Template);

            // TODO: THIS IS A MEGA-HACK, REMOVE AS SOON AS POSSIBLE!!!
            foreach (var key in scope.Variables) {
                if(key.StartsWith("__") && key.EndsWith("__")) {
                    templateScope.SetVariable(key, scope.GetVariable(key));
                }
            }

            if (arguments.Length > 1) {
                var parameters = (Core.Javascript.Object)arguments[1];
                foreach (var key in parameters.GetKeys()) {
                    templateScope.SetVariable(key, parameters.Get(key));
                }
            }

            return node.Execute(templateScope).Constant;
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
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            if (scope.ScopeParent == null) throw new InternalError("No scope to import code in");
            return node.Execute(scope.ScopeParent).Constant;
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