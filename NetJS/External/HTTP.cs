using NetJS.Core.Javascript;

namespace NetJS.External {
    /// <summary>HTTP class handles HTTP methods for clients.</summary>
    /// <remarks>This class can create GET, POST and custom requests.</remarks>
    class HTTP {

        /// <summary>Executes a GET method and returns the response.</summary>
        /// <param name="url">the url to fire the GET method at</param>
        /// <returns>a string with a response.</returns>
        /// <example><code lang="javascript">var response = HTTP.get("https://google.com");</code></example>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Core.Javascript.String)arguments[0]).Value;
            return new Core.Javascript.String(Util.HTTP.Get(url));
        }

        /// <summary>Executes a POST method and returns the response.</summary>
        /// <param name="url">The url to fire the POST method at</param>
        /// <param name="body">A body to attach to the POST method</param>
        /// <returns>a string with a response.</returns>
        /// <example><code lang="javascript">HTTP.post("https://google.com", {name: "newUser"}.ToString());</code></example>
        public static Constant post(Constant _this, Constant[] arguments, Scope scope) {
            var url = ((Core.Javascript.String)arguments[0]).Value;
            var content = ((Core.Javascript.String)arguments[1]).Value;
            return new Core.Javascript.String(Util.HTTP.Post(url, content));
        }

        /// <summary>Executes a GET method with a query.</summary>
        /// <param name="connectionName">A connection known in the application connections</param>
        /// <param name="query">The query to attach to the url</param>
        /// <returns>Returns the response, as a json object if possible.</returns>
        /// <example><code lang="javascript">HTTP.execute("google search", "q=hello+world");</code></example>
        /// <exception cref="InternalError">Thrown if application not found in application scope.</exception>
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