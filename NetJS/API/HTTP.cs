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
        ///     },
        ///     output: "text|binary|base64"
        /// });</code></example>
        public static dynamic execute(string connectionName, string query, object settings = null) {
            var application = State.Application;
            var url = application.Connections.GetHttpUrl(connectionName);
            return executeUrl(url + query, settings);
        }

        public static dynamic executeUrl(string url, dynamic settings = null) {
            var application = State.Application;
            var state = State.Get();

            var output = "text";

            return Tool.CreatePromise((resolve, reject) => {
                try {
                    var request = WebRequest.CreateHttp(url);

                    if (settings != null) {
                        if (Tool.GetObject(settings.cookies, out ScriptObject cookies)) {
                            request.CookieContainer = new CookieContainer();

                            foreach (var key in cookies.GetDynamicMemberNames()) {
                                // TODO: set path and host
                                request.CookieContainer.Add(new Cookie(key.ToString(), Tool.GetValue(cookies, key).ToString(), "/"));
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
                            var bytes = Encoding.Default.GetBytes(content);
                            request.ContentLength = bytes.Length;
                            using (var s = request.GetRequestStream()) {
                                s.Write(bytes, 0, bytes.Length);
                            }
                        }

                        if (Tool.Get(settings.output, out string outputType)) {
                            output = outputType;
                        }
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    var stream = response.GetResponseStream();

                    if (output == "text") {
                        application.AddCallback(resolve, ReadString(response), state);
                    } else if (output == "base64") {
                        var bytes = ReadFully(stream);
                        application.AddCallback(resolve, Convert.ToBase64String(bytes), state);
                    } else if (output == "bytes") {
                        var bytes = ReadFully(stream);
                        application.AddCallback(resolve, Tool.ToByteArray(bytes), state);
                    }
                } catch (Exception e) {
                    application.AddCallback(reject, $"Failed to get response from '{url}' {e.ToString()}", state);
                }
            });
        }

        // See: https://stackoverflow.com/a/9841920
        private static string ReadString (HttpWebResponse response) {
            string strWebPage = "";

            string Charset = response.CharacterSet ?? "utf-8";
            Encoding encoding = Encoding.GetEncoding(Charset);

            // read response into memory stream
            MemoryStream memoryStream;
            using (Stream responseStream = response.GetResponseStream()) {
                memoryStream = new MemoryStream();
                responseStream.CopyTo(memoryStream);
            }

            // set stream position to beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(memoryStream, encoding);
            strWebPage = sr.ReadToEnd();

            // Check real charset meta-tag in HTML
            int CharsetStart = strWebPage.IndexOf("charset=");
            if (CharsetStart > 0) {
                CharsetStart += 8;
                int CharsetEnd = strWebPage.IndexOfAny(new[] { ' ', '\"', ';' }, CharsetStart);
                string RealCharset =
                       strWebPage.Substring(CharsetStart, CharsetEnd - CharsetStart) ?? "utf-8";

                // real charset meta-tag in HTML differs from supplied server header???
                if (RealCharset != Charset) {
                    // get correct encoding
                    Encoding CorrectEncoding = Encoding.GetEncoding(RealCharset);

                    // reset stream position to beginning
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // reread response stream with the correct encoding
                    StreamReader sr2 = new StreamReader(memoryStream, CorrectEncoding);

                    strWebPage = sr2.ReadToEnd();

                    // Close and clean up the StreamReader
                    sr2.Close();
                }
            }

            // dispose the first stream reader object
            sr.Close();

            return strWebPage;
        }

        // See: https://stackoverflow.com/a/221941
        private static byte[] ReadFully(Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream()) {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
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