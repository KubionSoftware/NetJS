using NetJS.Core;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace NetJS.API {
    /// <summary>Sends HTTP requests.</summary>
    /// <configuration>When using a connection from connections.json, a connection needs to be defined as following:
    /// <example><code lang=json>{"jsonplaceholder": {"type": "http", "url": "https://jsonplaceholder.typicode.com"}}</code></example></configuration>
    class HTTP {

        /// <summary>Executes a HTTP request.</summary>
        /// <param name="connectionName">A connection from the connections.json or an url</param>
        /// <param name="query">The query to attach to the url (only usable when connectionName is not an url)</param>
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
        public static Constant execute(Constant _this, Constant[] arguments, Agent agent) {
            var connectionName = Core.Tool.GetArgument<Core.String>(arguments, 0, "HTTP.execute").Value;
            string url;
            string query = "";
            int settingsIndex = 1;

            if (connectionName.ToLower().StartsWith("http")) {
                url = connectionName;
            } else {
                var application = (agent as NetJSAgent).Application;
                url = application.Connections.GetHttpUrl(connectionName);

                query = Core.Tool.GetArgument<Core.String>(arguments, 1, "HTTP.execute").Value;
                settingsIndex++;
            }

            var request = WebRequest.CreateHttp(url + query);

            var settings = Core.Tool.GetArgument<Core.Object>(arguments, settingsIndex, "HTTP.execute", false);
            if(settings != null) {
                if (settings.Get("cookies") is Core.Object cookies) {
                    foreach (var key in cookies.OwnPropertyKeys()) {
                        request.CookieContainer.Add(new Cookie(key.ToString(), Core.Convert.ToString(cookies.Get(key), agent)));
                    }
                }

                if (settings.Get("headers") is Core.Object headers) {
                    foreach (var key in headers.OwnPropertyKeys()) {
                        Util.HttpWebRequestExtensions.SetRawHeader(request, key.ToString(), Core.Convert.ToString(headers.Get(key), agent));
                    }
                }

                if (settings.Get("method") is Core.String method) {
                    request.Method = method.Value;
                } else { 
                    request.Method = "GET";
                }

                if (settings.Get("content") is Core.String content) {
                    var writer = new StreamWriter(request.GetRequestStream(), Encoding.Default);
                    writer.Write(content);
                    writer.Close();
                }
            }

            try {
                var response = request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                var result = new Core.String(reader.ReadToEnd());
                reader.Close();

                if (response.ContentType.ToLower() == "application/json") {
                    try {
                        var json = Core.API.JSONAPI.parse(_this, new[] { result }, agent);
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