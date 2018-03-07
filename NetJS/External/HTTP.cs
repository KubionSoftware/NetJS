using NetJS.Core.Javascript;

namespace NetJS.External {
    class HTTP {

        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.HTTP.Get(url));
        }

        public static Constant post(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Core.Javascript.String)arguments[0]).Value;
            var content = ((Core.Javascript.String)arguments[1]).Value;
            return new Core.Javascript.String(Util.HTTP.Post(url, content));
        }

        public static Constant execute(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = ((Core.Javascript.String)arguments[0]).Value;

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var url = application.Connections.GetHttpUrl(connectionName);

            var query = ((Core.Javascript.String)arguments[1]).Value;

            var result = new Core.Javascript.String(Util.HTTP.Get(url + query));
            try {
                var json = Core.Internal.JSON.parse(_this, new[] { result }, scope);
                return json;
            } catch {
                return result;
            }
        }
    }
}