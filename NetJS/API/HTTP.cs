using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace NetJS.API {
    /// <summary>Sends HTTP requests.</summary>
    /// <configuration>When using a connection from connections.json, a connection needs to be defined as following:
    /// <example><code lang=json>{"jsonplaceholder": {"type": "http", "url": "https://jsonplaceholder.typicode.com"}}</code></example></configuration>
    public class HTTP {

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
        public static dynamic execute(string connectionName, string query, object settings = null) {
            var application = State.Application;
            var url = application.Connections.GetHttpUrl(connectionName);
            return executeUrl(url + query, settings);
        }

        public static dynamic executeUrl(string url, dynamic settings = null) {
            var application = State.Application;
            var state = State.Get();

            return Tool.CreatePromise((resolve, reject) => {
                var request = WebRequest.CreateHttp(url);

                if (settings != null) {
                    if (Tool.GetObject(settings.cookies, out ScriptObject cookies)) {
                        foreach (var key in cookies.GetDynamicMemberNames()) {
                            request.CookieContainer.Add(new Cookie(key.ToString(), Tool.GetValue(cookies, key).ToString()));
                        }
                    }

                    if (Tool.GetObject(settings.headers, out ScriptObject headers)) {
                        foreach (var key in headers.GetDynamicMemberNames()) {
                            Util.HttpWebRequestExtensions.SetRawHeader(request, key.ToString(), Tool.GetValue(headers, key).ToString());
                        }
                    }

                    if (Tool.Get(settings.method, out string method)) {
                        request.Method = method;
                    } else {
                        request.Method = "GET";
                    }

                    if (Tool.Get(settings.content, out string content)) {
                        var writer = new StreamWriter(request.GetRequestStream(), Encoding.Default);
                        writer.Write(content);
                        writer.Close();
                    }
                }

                try {
                    var response = request.GetResponse();

                    var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    var result = reader.ReadToEnd();
                    reader.Close();

                    if (response.ContentType.ToLower() == "application/json") {
                        // TODO: parse json
                    }

                    application.AddCallback(resolve, result, state);
                } catch (Exception e) {
                    application.AddCallback(reject, $"Failed to get response from '{url}' {e.Message}", state);
                }
            });
        }

        public static dynamic get(string url) {
            return executeUrl(url);
        }

        public static dynamic post(string url, string content) {
            return execute(url, Tool.ToObject(new Dictionary<string, object>() {
                { "method", "POST" },
                { "content", content }
            }));
        }
    }
}