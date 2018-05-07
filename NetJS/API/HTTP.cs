using NetJS.Core.Javascript;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace NetJS.API {
    /// <summary>Sends HTTP requests.</summary>
    class HTTP {

        /// <summary>Executes a HTTP request.</summary>
        /// <param name="connectionName">A connection from the connections.json</param>
        /// <param name="query">The query to attach to the url</param>
        /// <param name="options">Optional settings: method (string), content (string), cookies (object), headers (object)</param>
        /// <returns>Returns the response, as a json object if response Content-Type is 'application/json'.</returns>
        /// <example><code lang="javascript">var result = HTTP.execute("REST", "articles", {
        ///     method: "POST",
        ///     content: JSON.stringify(article),
        ///     cookies: {
        ///         UserID: "23433"
        ///     },
        ///     headers: {
        ///         ApplicationID: "NetJS"
        ///     }
        /// });</code></example>
        /// <exception cref="Error">Thrown if the request failed</exception>
        public static Constant execute(Constant _this, Constant[] arguments, Scope scope) {
            var connectionName = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "HTTP.execute").Value;
            string url;
            string query = "";
            int settingsIndex = 1;

            if (connectionName.ToLower().StartsWith("http")) {
                url = connectionName;
            } else {
                var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
                if (application == null) throw new InternalError("No application");

                url = application.Connections.GetHttpUrl(connectionName);

                query = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "HTTP.execute").Value;
                settingsIndex++;
            }

            var request = WebRequest.CreateHttp(url + query);

            var settings = Core.Tool.GetArgument<Core.Javascript.Object>(arguments, settingsIndex, "HTTP.execute", false);
            if(settings != null) {
                if (settings.Get("cookies") is Core.Javascript.Object cookies) {
                    foreach (var key in cookies.GetKeys()) {
                        request.CookieContainer.Add(new Cookie(key, Core.Tool.ToString(cookies.Get(key), scope)));
                    }
                }

                if (settings.Get("headers") is Core.Javascript.Object headers) {
                    foreach (var key in headers.GetKeys()) {
                        Util.HttpWebRequestExtensions.SetRawHeader(request, key, Core.Tool.ToString(headers.Get(key), scope));
                    }
                }

                if (settings.Get("method") is Core.Javascript.String method) {
                    request.Method = method.Value;
                } else { 
                    request.Method = "GET";
                }

                if (settings.Get("content") is Core.Javascript.String content) {
                    var writer = new StreamWriter(request.GetRequestStream(), Encoding.Default);
                    writer.Write(content);
                    writer.Close();
                }
            }

            try {
                var response = request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                var result = new Core.Javascript.String(reader.ReadToEnd());
                reader.Close();

                if (response.ContentType.ToLower() == "application/json") {
                    try {
                        var json = Core.API.JSON.parse(_this, new[] { result }, scope);
                        return json;
                    } catch { }
                }

                return result;
            } catch {
                throw new Error($"HTTP Error - Failed to get response from '{url}'");
            }
        }
    }
}