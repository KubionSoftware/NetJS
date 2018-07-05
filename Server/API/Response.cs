using Microsoft.ClearScript.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server.API {
    public class Response {

        /// <summary>Writes a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <param name="value">The value to set (string)</param>
        /// <param name="options">Options: expires (date), path (string) and httpOnly (boolean)</param>
        /// <example><code lang="javascript">Response.setCookie("SSID", "AE3oaD8COGojttJue", {
        ///     expires: new Date(),
        ///     path: "localhost/example",
        ///     httpOnly: false
        /// });</code></example>
        public static void setCookie(string key, string value, dynamic options = null) {
            var cookie = new HttpCookie(key, value);

            if (options != null) {
                if(options.expires is DateTime date) {
                    cookie.Expires = date;
                }

                if (options.path is string path) {
                    cookie.Path = path;
                }

                if (options.httpOnly is bool httpOnly) {
                    cookie.HttpOnly = httpOnly;
                }
            }
            
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>Removes a cookie.</summary>
        /// <param name="name">The name of the cookie (string)</param>
        /// <example><code lang="javascript">Response.removeCookie("SSID")</code></example>
        public static void removeCookie(string key) {
            var cookie = new HttpCookie(key);
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>Writes an header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <param name="value">The value to set (string)</param>
        /// <example><code lang="javascript">Response.setHeader("Content-Type", "application/json");</code></example>
        public static void setHeader(string key, string value) {
            var context = Tool.GetContext();
            context.Response.AppendHeader(key, value);
        }

        /// <summary>Removes an header.</summary>
        /// <param name="name">The name of the header (string)</param>
        /// <example><code lang="javascript">Response.removeHeader("ApplicationID")</code></example>
        public static void removeHeader(string key) {
            var context = Tool.GetContext();
            context.Response.Headers.Remove(key);
        }

        /// <summary>Sends an array of bytes and exits the execution immediately</summary>
        /// <param name="bytes">The bytes to send (Uint8Array)</param>
        /// <example><code lang="javascript">Request.sendBytes(bytes).then(success => {
        ///     if (success) { ... }
        /// });</code></example>
        public static dynamic sendBytes(dynamic bytes) {
            var application = State.Application;
            var state = State.Get();
            var context = Tool.GetContext();

            return NetJS.Tool.CreatePromise((resolve, reject) => {
                var content = ((IArrayBuffer)bytes.buffer).GetBytes();
                context.Response.OutputStream.Write(content, 0, content.Length);
                context.Response.OutputStream.Close();
                context.Response.End();

                application.AddCallback(resolve, true, state);
            });
        }

        /// <summary>Sends an file and exits the execution immediately</summary>
        /// <param name="name">The name of the file (string)</param>
        /// <example><code lang="javascript">Request.sendFile("image.png").then(success => {
        ///     if (success) { ... }
        /// });</code></example>
        public static dynamic sendFile(string name) {
            var application = State.Application;
            var state = State.Get();
            var context = Tool.GetContext();

            return NetJS.Tool.CreatePromise((resolve, reject) => {
                var file = application.Cache.GetPath(name, application, false);
                context.Response.ContentType = MimeMapping.GetMimeMapping(file);
                context.Response.TransmitFile(file);
                context.Response.End();

                application.AddCallback(resolve, true, state);
            });
        }

        /// <summary>Sets the response status code.</summary>
        /// <param name="statusCode">The status code (number)</param>
        /// <example><code lang="javascript">Response.setStatusCode(200);</code></example>
        public static void setStatusCode(int code) {
            var context = Tool.GetContext();
            context.Response.StatusCode = code;
        }

        /// <summary>Sets the response status description.</summary>
        /// <param name="statusDescription">The status description (number)</param>
        /// <example><code lang="javascript">Response.setStatusDescription(200);</code></example>
        public static void setStatusDescription(string description) {
            var context = Tool.GetContext();
            context.Response.StatusDescription = description;
        }

        /// <summary>Sets the response encoding.</summary>
        /// <param name="encoding">The web name (registered with IANA) of the encoding (string)</param>
        /// <example><code lang="javascript">Response.setEncoding("UTF-8");</code></example>
        public static void setEncoding(string encoding) {
            var context = Tool.GetContext();
            context.Response.ContentEncoding = Encoding.GetEncoding(encoding);
        }
    }
}