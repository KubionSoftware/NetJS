namespace NetJS.External {
    class HTTP {

        public static Javascript.Constant get(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var url = ((Javascript.String)arguments[0]).Value;
            return new Javascript.String(Util.HTTP.Get(url));
        }

        public static Javascript.Constant post(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var url = ((Javascript.String)arguments[0]).Value;
            var content = ((Javascript.String)arguments[1]).Value;
            return new Javascript.String(Util.HTTP.Post(url, content));
        }

        public static Javascript.Constant execute(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var connectionName = ((Javascript.String)arguments[0]).Value;
            var url = scope.Application.Connections.GetHttpUrl(connectionName);

            var query = ((Javascript.String)arguments[1]).Value;

            var result = new Javascript.String(Util.HTTP.Get(url + query));
            try {
                var json = Internal.JSON.parse(_this, new[] { result }, scope);
                return json;
            } catch {
                return result;
            }
        }
    }
}