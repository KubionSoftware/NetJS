using NetJS.Core.Javascript;
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
        public static Constant getCookie(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Request.getCookie").Value;

            var cookie = HttpContext.Current.Request.Cookies.Get(key);
            return new Core.Javascript.String(cookie.Value);
        }

        /// <summary>Reads all cookies.</summary>
        /// <returns>An object with the keys being the cookie names and the values the cookie values</returns>
        /// <example><code lang="javascript">var cookies = Response.getCookies();</code></example>
        public static Constant getCookies(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var cookies = Core.Tool.Construct("Object", lex);

            foreach(HttpCookie cookie in HttpContext.Current.Request.Cookies) {
                cookies.Set(cookie.Name, new Core.Javascript.String(cookie.Value));
            }
            
            return cookies;
        }

        /// <summary>Reads a request header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <returns>The header value (string)</returns>
        /// <example><code lang="javascript">var acceptedTypes = Request.getHeader("Accept");</code></example>
        public static Constant getHeader(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Request.getHeader").Value;

            var context = Tool.GetContext(lex, "Request.getHeader");
            var header = context.Request.Headers.Get(key);
            return new Core.Javascript.String(header);
        }

        /// <summary>Reads all headers.</summary>
        /// <returns>An object with the keys being the header names and the values the header values</returns>
        /// <example><code lang="javascript">var headers = Request.getHeaders();</code></example>
        public static Constant getHeaders(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var headers = Core.Tool.Construct("Object", lex);

            var context = Tool.GetContext(lex, "Request.getHeaders");
            foreach (var key in context.Request.Headers.AllKeys) {
                headers.Set(key, new Core.Javascript.String(context.Request.Headers[key]));
            }

            return headers;
        }

        /// <summary>Gets the requested URL</summary>
        /// <returns>The url (string)</returns>
        /// <example><code lang="javascript">var url = Request.getUrl();</code></example>
        public static Constant getUrl(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getUrl");
            var url = context.Request.Url.ToString();
            return new Core.Javascript.String(url);
        }

        /// <summary>Gets the requested path</summary>
        /// <returns>The path (string[])</returns>
        /// <example><code lang="javascript">var path = Request.getPath();</code></example>
        public static Constant getPath(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getPath");
            var path = Tool.GetPath(context.Request);
            return Core.Tool.ToArray(path, lex);
        }

        /// <summary>Gets a parameter from the query part of the url</summary>
        /// <param name="name">The name of the parameter (string)</param>
        /// <returns>The value (string)</returns>
        /// <example><code lang="javascript">var value = Request.getParameter("q");</code></example>
        public static Constant getParameter(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "Request.getParameter").Value;

            var context = Tool.GetContext(lex, "Request.getParameter");
            var value = context.Request.QueryString[key] ?? "";
            return new Core.Javascript.String(value);
        }

        /// <summary>Gets all parameters from the query part of the url</summary>
        /// <returns>An object with the keys being the parameter names and the values being the parameter values (string)</returns>
        /// <example><code lang="javascript">var parameters = Request.getParameters();</code></example>
        public static Constant getParameters(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var parameters = Core.Tool.Construct("Object", lex);

            var context = Tool.GetContext(lex, "Request.getParameters");
            foreach (var key in context.Request.QueryString.AllKeys) {
                parameters.Set(key, new Core.Javascript.String(context.Request.QueryString[key]));
            }

            return parameters;
        }

        /// <summary>Gets the content of the request</summary>
        /// <returns>The content (string)</returns>
        /// <example><code lang="javascript">var content = Request.getContent();</code></example>
        public static Constant getContent(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getParameter");
            context.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
            var content = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            return new Core.Javascript.String(content);
        }

        /// <summary>Gets the encoding of the request content</summary>
        /// <returns>The web name (registered with IANA) of the encoding (string)</returns>
        /// <example><code lang="javascript">var encoding = Request.getEncoding();</code></example>
        public static Constant getEncoding(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getEncoding");
            var encoding = context.Request.ContentEncoding.WebName;
            return new Core.Javascript.String(encoding);
        }

        /// <summary>Gets the method of the request</summary>
        /// <returns>The method (string)</returns>
        /// <example><code lang="javascript">var method = Request.getMethod();</code></example>
        public static Constant getMethod(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getMethod");
            var method = context.Request.HttpMethod;
            return new Core.Javascript.String(method);
        }

        /// <summary>Checks if the request is secure (via HTTPS)</summary>
        /// <returns>If the request is secure (boolean)</returns>
        /// <example><code lang="javascript">var secure = Request.isSecure();</code></example>
        public static Constant isSecure(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.isSecure");
            var secure = context.Request.IsSecureConnection;
            return new Core.Javascript.Boolean(secure);
        }

        /// <summary>Gets the form content of the request</summary>
        /// <returns>An object with the keys being the field names and the values being the field values</returns>
        /// <example><code lang="javascript">var form = Request.getForm();</code></example>
        public static Constant getForm(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var form = Core.Tool.Construct("Object", lex);

            var context = Tool.GetContext(lex, "Request.getForm");
            foreach (var key in context.Request.Form.AllKeys) {
                form.Set(key, new Core.Javascript.String(context.Request.Form[key]));
            }

            return form;
        }

        /// <summary>Gets the sessionId from IIS</summary>
        /// <returns>The session id, a unique string for each different session</returns>
        /// <example><code lang="javascript">var sessionId = Request.getSessionId();</code></example>
        public static Constant getSessionId(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getSessionId");
            var sessionId = context.Session != null ? context.Session.SessionID : "";
            return new Core.Javascript.String(sessionId);
        }

        /// <summary>Gets information about the user IP and agent</summary>
        /// <returns>An object with {
        ///     ip: "192.0.168.2",
        ///     agent: "Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) ..."
        /// }</returns>
        /// <example><code lang="javascript">var user = Request.getUser();</code></example>
        public static Constant getUser(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var user = Core.Tool.Construct("Object", lex);

            var context = Tool.GetContext(lex, "Request.getUser");
            user.Set("ip", new Core.Javascript.String(context.Request.UserHostAddress));
            user.Set("agent", new Core.Javascript.String(context.Request.UserAgent));

            return user;
        }

        /// <summary>Gets the number of files in the request</summary>
        /// <returns>The file count (number)</returns>
        /// <example><code lang="javascript">var fileCount = Request.getFileCount();</code></example>
        public static Constant getFileCount(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var context = Tool.GetContext(lex, "Request.getFileCount");
            var fileCount = context.Request.Files.Count;
            return new Core.Javascript.Number(fileCount);
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
        public static Constant getFile(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var index = Core.Tool.GetArgument<Core.Javascript.Number>(arguments, 0, "Request.getFile");

            var context = Tool.GetContext(lex, "Request.getFile");
            var result = Core.Tool.Construct("Object", lex);

            var file = context.Request.Files[(int)index.Value];
            result.Set("name", new Core.Javascript.String(file.FileName));
            result.Set("contentType", new Core.Javascript.String(file.ContentType));
            result.Set("size", new Core.Javascript.Number(file.ContentLength));

            using (MemoryStream ms = new MemoryStream()) {
                file.InputStream.CopyTo(ms);
                result.Set("content", new Core.Javascript.Uint8Array(new Core.Javascript.ArrayBuffer(ms.ToArray())));
            }

            return result;
        }

        /// <summary>Saves a file to disk</summary>
        /// <param name="index">The index of the file (number)</param>
        /// <param name="name">The name of the new file (string)</param>
        /// <example><code lang="javascript">Request.saveFile(0, "image.png");</code></example>
        public static Constant saveFile(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var index = Core.Tool.GetArgument<Core.Javascript.Number>(arguments, 0, "Request.saveFile");
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "Request.saveFile");

            var application = NetJS.Tool.GetApplication(lex);

            var context = Tool.GetContext(lex, "Request.saveFile");
            context.Request.Files[(int)index.Value].SaveAs(application.Cache.GetPath(name.Value, application, false));
            return Static.Undefined;
        }
    }
}