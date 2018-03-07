using System.Web;
using NetJS.Core.Javascript;

namespace NetJS.External {
    public class Functions {

        public static Constant include(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "include");
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            var templateScope = new Scope(application.Engine.Scope, node, ScopeType.Template);

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

        public static Constant import(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "import");
            if (!name.Value.EndsWith(".js")) {
                name.Value += ".js";
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var node = application.Cache.GetFile(name.Value, application);

            if (scope.Parent == null) throw new InternalError("No scope to import code in");
            return node.Execute(scope.Parent).Constant;
        }

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