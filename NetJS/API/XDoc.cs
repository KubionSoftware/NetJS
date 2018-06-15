using System.Collections;
using System.Web;
using NetJS.Core;

namespace NetJS.API {
    /// <summary>A Compatibility class for XDoc, to ensure possibility of usage of XDoc with NetJS.</summary>
    public class XDoc {

        public class XDocInfo {
            public XHTMLMerge.AppCache AppCache;
            public XHTMLMerge.SVCache SVCache;
        }

        public static XDocInfo GetXDocInfo(Agent agent) {
            var context = HttpContext.Current;

            XHTMLMerge.AppCache appCache = null;
            XHTMLMerge.SVCache svCache = null;

            if (context != null) {
                if (context.Application != null) appCache = (XHTMLMerge.AppCache)context.Application["AppCache"];
                if (context.Session != null) svCache = (XHTMLMerge.SVCache)context.Session["SVCache"];
            } else {
                var application = (agent as NetJSAgent).Application;
                var session = (agent as NetJSAgent).Session;

                if (application.Get("AppCache") is Foreign af) appCache = (XHTMLMerge.AppCache)af.Value;
                if (session.Get("SVCache") is Foreign sf) svCache = (XHTMLMerge.SVCache)sf.Value;
            }

            if (appCache == null) appCache = new XHTMLMerge.AppCache();
            if (svCache == null) svCache = new XHTMLMerge.SVCache();

            return new XDocInfo() { AppCache = appCache, SVCache = svCache };
        }

        public static void SetXDocInfo(XDocInfo xdoc, Agent agent) {
            var context = HttpContext.Current;

            if (context != null) {
                if (context.Application != null) context.Application["AppCache"] = xdoc.AppCache;
                if (context.Session != null) context.Session["SVCache"] = xdoc.SVCache;
            } else {
                var application = (agent as NetJSAgent).Application;
                var session = (agent as NetJSAgent).Session;

                application.Set("AppCache", new Foreign(xdoc.AppCache));
                session.Set("SVCache", new Foreign(xdoc.SVCache));
            }
        }

        private static string includeLoad(Constant _this, Constant[] arguments, Agent agent) {
            var name = Core.Tool.GetArgument<Core.String>(arguments, 0, "XDoc.include");

            var xdoc = GetXDocInfo(agent);

            var parameters = new Hashtable();
            if (arguments.Length > 1) {
                var param = (Core.Object)arguments[1];
                foreach (var key in param.OwnPropertyKeys()) {
                    parameters.Add(key.ToString(), Convert.ToString(param.Get(key), agent));
                }
            }

            var application = (agent as NetJSAgent).Application;

            var context = HttpContext.Current;
            var result = application.XDocService.RunTemplate(context, name.Value, parameters, ref xdoc.AppCache, ref xdoc.SVCache);

            return result;
        }

        /// <summary>XDoc.include includes an XDoc template and writes the result to the buffer</summary>
        /// <param name="name">Name of the included file</param>
        /// <param name="parameters">optional, 0 or more parameters to be set before executing the template</param>
        /// <returns>Undefined</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in the application agent.</exception>
        public static Constant include(Constant _this, Constant[] arguments, Agent agent) {
            agent.Running.Buffer.Append(includeLoad(_this, arguments, agent));
            return Static.Undefined;
        }

        /// <summary>XDoc.load includes an XDoc template and returns the result.</summary>
        /// <param name="name">Name of the included file</param>
        /// <param name="parameters">optional, 0 or more parameters to be set before executing the template</param>
        /// <returns>The result of executing the file.</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in the application agent.</exception>
        public static Constant load(Constant _this, Constant[] arguments, Agent agent) {
            return new String(includeLoad(_this, arguments, agent));
        }

        /// <summary>XDoc.get gets a value from the XDoc session</summary>
        /// <param name="key">Value name</param>
        /// <param name="context">Context name</param>
        /// <param name="id">ID name</param>
        /// <returns>The session value as a string</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in application agent.</exception>
        public static Constant get(Constant _this, Constant[] arguments, Agent agent) {
            var key = Core.Tool.GetArgument<Core.String>(arguments, 0, "XDoc.get").Value;
            var context = Core.Tool.GetArgument<Core.String>(arguments, 1, "XDoc.get").Value;
            var id = Core.Tool.GetArgument<Core.String>(arguments, 2, "XDoc.get").Value;

            var xdoc = GetXDocInfo(agent);
            var value = xdoc.SVCache.GetSV(key, context + "_" + id, "");
            SetXDocInfo(xdoc, agent);

            if (value.Length == 0) return Core.Static.Undefined;
            return new Core.String(value);
        }

        /// <summary>XDoc.set sets a value in the XDoc session</summary>
        /// <param name="key">Value name</param>
        /// <param name="context">Context name</param>
        /// <param name="id">ID name</param>
        /// <param name="value">The value to set, is converted to a string</param>
        /// <returns>Undefined</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in application agent.</exception>
        public static Constant set(Constant _this, Constant[] arguments, Agent agent) {
            var key = Core.Tool.GetArgument<Core.String>(arguments, 0, "XDoc.get").Value;
            var context = Core.Tool.GetArgument<Core.String>(arguments, 1, "XDoc.get").Value;
            var id = Core.Tool.GetArgument<Core.String>(arguments, 2, "XDoc.get").Value;
            var value = Core.Tool.GetArgument(arguments, 3, "Session.set");

            var xdoc = GetXDocInfo(agent);
            xdoc.SVCache.SetSV(key, context + "_" + id, value.ToString());
            SetXDocInfo(xdoc, agent);

            return Static.Undefined;
        }
    }
}