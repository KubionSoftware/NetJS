using Microsoft.ClearScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace NetJS.API {
    /// <summary>A Compatibility class for XDoc, to ensure possibility of usage of XDoc with NetJS.</summary>
    public class XDoc {

        private static dynamic _onMessage;

        public class XDocInfo {
            public XHTMLMerge.AppCache AppCache;
            public XHTMLMerge.SVCache SVCache;
        }

        public static XDocInfo GetXDocInfo() {
            var context = HttpContext.Current;

            XHTMLMerge.AppCache appCache = null;
            XHTMLMerge.SVCache svCache = null;

            if (context != null) {
                if (context.Application != null) appCache = (XHTMLMerge.AppCache)context.Application["AppCache"];
                if (context.Session != null) svCache = (XHTMLMerge.SVCache)context.Session["SVCache"];
            } else {
                var application = State.Application;
                var session = State.Session;

                if (application.Get("AppCache") is object af) appCache = (XHTMLMerge.AppCache)af;
                if (session.Get("SVCache") is object sf) svCache = (XHTMLMerge.SVCache)sf;
            }

            if (appCache == null) appCache = new XHTMLMerge.AppCache();
            if (svCache == null) svCache = new XHTMLMerge.SVCache();

            return new XDocInfo() { AppCache = appCache, SVCache = svCache };
        }

        public static void SetXDocInfo(XDocInfo xdoc) {
            var context = HttpContext.Current;

            if (context != null) {
                if (context.Application != null) context.Application["AppCache"] = xdoc.AppCache;
                if (context.Session != null) context.Session["SVCache"] = xdoc.SVCache;
            } else {
                var application = State.Application;
                var session = State.Session;

                application.Set("AppCache", xdoc.AppCache);
                session.Set("SVCache", xdoc.SVCache);
            }
        }

        /// <summary>Creates an event listener</summary>
        /// <param name="event">The name of the event (connection)</param>
        /// <param name="func">The function to call</param>
        /// <returns>undefined</returns>
        /// <example><code lang="javascript">XDoc.on("message", function(data){
        ///     Log.write(data);
        /// });</code></example>
        public static void on(string e, dynamic f) {
            if (e == "message") {
                _onMessage = f;
            }
        }

        public static void Send(JSApplication application, JSSession session, Action<object> callback, string data) {
            if (_onMessage == null) return;
            
            var request = new FunctionRequest(_onMessage, application, callback, session, data);
            application.AddRequest(request);
        }

        public static void ResetHooks() {
            _onMessage = null;
        }

        /// <summary>XDoc.get gets a value from the XDoc session</summary>
        /// <param name="key">Value name</param>
        /// <param name="context">Context name</param>
        /// <param name="id">ID name</param>
        /// <returns>The session value as a string</returns>
        public static string get(string key, string context, string id) {
            var xdoc = GetXDocInfo();
            var value = xdoc.SVCache.GetSV(key, context + "_" + id, "");
            SetXDocInfo(xdoc);

            if (value.Length == 0) return "";
            return value;
        }

        /// <summary>XDoc.set sets a value in the XDoc session</summary>
        /// <param name="key">Value name</param>
        /// <param name="context">Context name</param>
        /// <param name="id">ID name</param>
        /// <param name="value">The value to set, is converted to a string</param>
        /// <returns>Undefined</returns>
        public static void set(string key, string context, string id, object value) {
            var xdoc = GetXDocInfo();
            xdoc.SVCache.SetSV(key, context + "_" + id, value.ToString());
            SetXDocInfo(xdoc);
        }

        private static string includeLoad(string name, ScriptObject arguments) {
            var xdoc = GetXDocInfo();

            var parameters = new Hashtable();
            if (arguments != null) {
                foreach (var key in arguments.GetDynamicMemberNames()) {
                    parameters.Add(key.ToString(), Tool.GetValue(arguments, key).ToString());
                }
            }

            var application = State.Application;

            var context = HttpContext.Current;
            var result = application.XDocService.RunTemplate(context, name, parameters, ref xdoc.AppCache, ref xdoc.SVCache);

            SetXDocInfo(xdoc);

            return result;
        }

        /// <summary>XDoc.include includes an XDoc template and writes the result to the buffer</summary>
        /// <param name="name">Name of the included file</param>
        /// <param name="parameters">optional, 0 or more parameters to be set before executing the template</param>
        /// <returns>Undefined</returns>
        public static void include(string name, dynamic arguments = null) {
            State.Buffer.Append(includeLoad(name, arguments));
        }

        /// <summary>XDoc.load includes an XDoc template and returns the result.</summary>
        /// <param name="name">Name of the included file</param>
        /// <param name="parameters">optional, 0 or more parameters to be set before executing the template</param>
        /// <returns>The result of executing the file.</returns>
        public static string load(string name, dynamic arguments = null) {
            return includeLoad(name, arguments);
        }
    }
}