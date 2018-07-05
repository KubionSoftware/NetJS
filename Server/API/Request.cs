using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NetJS.Server.API {
    public class Request {

        /// <summary>Reads a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <returns>The cookie value (string)</returns>
        /// <example><code lang="javascript">var ssid = Request.getCookie("SSID");</code></example>
        public static string getCookie(string key) {
            var cookie = HttpContext.Current.Request.Cookies.Get(key);
            return cookie.Value;
        }

        /// <summary>Reads all cookies.</summary>
        /// <returns>An object with the keys being the cookie names and the values the cookie values</returns>
        /// <example><code lang="javascript">var cookies = Response.getCookies();</code></example>
        public static dynamic getCookies() {
            var cookies = new Dictionary<string, object>();

            foreach (HttpCookie cookie in HttpContext.Current.Request.Cookies) {
                cookies.Add(cookie.Name, cookie.Value);
            }

            return NetJS.Tool.ToObject(cookies);
        }

        /// <summary>Reads a request header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <returns>The header value (string)</returns>
        /// <example><code lang="javascript">var acceptedTypes = Request.getHeader("Accept");</code></example>
        public static string getHeader(string key) {
            var context = Tool.GetContext();
            var header = context.Request.Headers.Get(key);
            return header;
        }

        /// <summary>Reads all headers.</summary>
        /// <returns>An object with the keys being the header names and the values the header values</returns>
        /// <example><code lang="javascript">var headers = Request.getHeaders();</code></example>
        public static dynamic getHeaders() {
            var headers = new Dictionary<string, object>();

            var context = Tool.GetContext();
            foreach (var key in context.Request.Headers.AllKeys) {
                headers.Add(key, context.Request.Headers[key]);
            }

            return NetJS.Tool.ToObject(headers);
        }

        /// <summary>Gets the requested URL</summary>
        /// <returns>The url (string)</returns>
        /// <example><code lang="javascript">var url = Request.getUrl();</code></example>
        public static string getUrl() {
            var context = Tool.GetContext();
            var url = context.Request.Url.ToString();
            return url;
        }

        /// <summary>Gets the requested path</summary>
        /// <returns>The path (string[])</returns>
        /// <example><code lang="javascript">var path = Request.getPath();</code></example>
        public static dynamic getPath() {
            var context = Tool.GetContext();
            var path = Tool.GetPath(context.Request);
            return NetJS.Tool.ToArray(path);
        }

        /// <summary>Gets a parameter from the query part of the url</summary>
        /// <param name="name">The name of the parameter (string)</param>
        /// <returns>The value (string)</returns>
        /// <example><code lang="javascript">var value = Request.getParameter("q");</code></example>
        public static string getParameter(string key) {
            var context = Tool.GetContext();
            var value = context.Request.QueryString[key] ?? "";
            return value;
        }

        /// <summary>Gets all parameters from the query part of the url</summary>
        /// <returns>An object with the keys being the parameter names and the values being the parameter values (string)</returns>
        /// <example><code lang="javascript">var parameters = Request.getParameters();</code></example>
        public static dynamic getParameters() {
            var parameters = new Dictionary<string, object>();

            var context = Tool.GetContext();
            foreach (var key in context.Request.QueryString.AllKeys) {
                parameters.Add(key, context.Request.QueryString[key]);
            }

            return NetJS.Tool.ToObject(parameters);
        }

        /// <summary>Gets the content of the request</summary>
        /// <returns>The content (string)</returns>
        /// <example><code lang="javascript">var content = Request.getContent();</code></example>
        public static string getContent() {
            var context = Tool.GetContext();
            context.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
            var content = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            return content;
        }

        /// <summary>Gets the encoding of the request content</summary>
        /// <returns>The web name (registered with IANA) of the encoding (string)</returns>
        /// <example><code lang="javascript">var encoding = Request.getEncoding();</code></example>
        public static string getEncoding() {
            var context = Tool.GetContext();
            var encoding = context.Request.ContentEncoding.WebName;
            return encoding;
        }

        /// <summary>Gets the method of the request</summary>
        /// <returns>The method (string)</returns>
        /// <example><code lang="javascript">var method = Request.getMethod();</code></example>
        public static string getMethod() {
            var context = Tool.GetContext();
            var method = context.Request.HttpMethod;
            return method;
        }

        /// <summary>Checks if the request is secure (via HTTPS)</summary>
        /// <returns>If the request is secure (boolean)</returns>
        /// <example><code lang="javascript">var secure = Request.isSecure();</code></example>
        public static bool isSecure() {
            var context = Tool.GetContext();
            var secure = context.Request.IsSecureConnection;
            return secure;
        }

        /// <summary>Gets the form content of the request</summary>
        /// <returns>An object with the keys being the field names and the values being the field values</returns>
        /// <example><code lang="javascript">var form = Request.getForm();</code></example>
        public static dynamic getForm() {
            var form = new Dictionary<string, object>();

            var context = Tool.GetContext();
            foreach (var key in context.Request.Form.AllKeys) {
                form.Add(key, context.Request.Form[key]);
            }

            return NetJS.Tool.ToObject(form);
        }

        /// <summary>Gets the sessionId from IIS</summary>
        /// <returns>The session id, a unique string for each different session</returns>
        /// <example><code lang="javascript">var sessionId = Request.getSessionId();</code></example>
        public static string getSessionId() {
            var context = Tool.GetContext();
            var sessionId = context.Session != null ? context.Session.SessionID : "";
            return sessionId;
        }

        /// <summary>Gets information about the user IP and agent</summary>
        /// <returns>An object with {
        ///     ip: "192.0.168.2",
        ///     agent: "Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) ..."
        /// }</returns>
        /// <example><code lang="javascript">var user = Request.getUser();</code></example>
        public static object getUser() {
            var user = new Dictionary<string, object>();

            var context = Tool.GetContext();
            user.Add("ip", context.Request.UserHostAddress);
            user.Add("agent", context.Request.UserAgent);

            return NetJS.Tool.ToObject(user);
        }

        /// <summary>Gets the number of files in the request</summary>
        /// <returns>The file count (number)</returns>
        /// <example><code lang="javascript">var fileCount = Request.getFileCount();</code></example>
        public static int getFileCount() {
            var context = Tool.GetContext();
            var fileCount = context.Request.Files.Count;
            return fileCount;
        }

        /// <summary>Gets a file from the request</summary>
        /// <param name="index">The index of the file (number)</param>
        /// <returns>An object with: {
        ///     content: [[Uint8Array]],
        ///     contentType: "image/png",
        ///     size: 35937, // bytes,
        ///     name: "image.png"
        /// }</returns>
        /// <example><code lang="javascript">var file = Request.getFile(0);</code></example>
        public static dynamic getFile(int index) {
            var context = Tool.GetContext();
            var result = new Dictionary<string, object>();

            var file = context.Request.Files[index];
            result.Add("name", file.FileName);
            result.Add("contentType", file.ContentType);
            result.Add("size", file.ContentLength);

            using (MemoryStream ms = new MemoryStream()) {
                file.InputStream.CopyTo(ms);
                result.Add("content", NetJS.Tool.ToByteArray(ms.ToArray()));
            }

            return result;
        }

        /// <summary>Saves a file to disk</summary>
        /// <param name="index">The index of the file (number)</param>
        /// <param name="name">The name of the new file (string)</param>
        /// <example><code lang="javascript">Request.saveFile(0, "image.png");</code></example>
        public static void saveFile(int index, string name) {
            var application = State.Application;

            var context = Tool.GetContext();
            context.Request.Files[index].SaveAs(application.Cache.GetPath(name, application, false));
        }
    }
}