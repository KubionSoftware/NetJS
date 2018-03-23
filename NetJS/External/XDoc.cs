using System.Collections;
using System.Web;
using NetJS.Core.Javascript;

namespace NetJS.External {
    /// <summary>A Compatibility class for XDoc, to ensure possibility of usage of XDoc with NetJS.</summary>
    public class XDoc {
        /// <summary>XDoc.include includes other XDoc templates and returns the result.</summary>
        /// <param name="name">Name of the included file</param>
        /// <param name="parameters">optional, 0 or more parameters to be set before executing the template</param>
        /// <returns>The result of executing the file.</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in the application scope.</exception>
        public static Constant include(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "XDoc.include");

            var context = HttpContext.Current;

            XHTMLMerge.AppCache appCache = null;
            XHTMLMerge.SVCache svCache = null;

            if (context != null && context.Application != null) appCache = (XHTMLMerge.AppCache)context.Application["AppCache"];
            if (appCache == null) appCache = new XHTMLMerge.AppCache();

            if (context != null && context.Session != null) svCache = (XHTMLMerge.SVCache)context.Session["SVCache"];
            if (svCache == null) svCache = new XHTMLMerge.SVCache();

            var parameters = new Hashtable();
            if (arguments.Length > 1) {
                var param = (Core.Javascript.Object)arguments[1];
                foreach (var key in param.GetKeys()) {
                    parameters.Add(key, param.Get(key).ToString());
                }
            }

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            var result = application.XDocService.RunTemplate(context, name.Value, parameters, ref appCache, ref svCache);

            if (context != null && context.Application != null) context.Application["AppCache"] = appCache;
            if (context != null && context.Session != null) context.Session["SVCache"] = svCache;

            return new Core.Javascript.String(result);
        }

        /// <summary>XDoc.get runs a svCache.GetSV with the given parameters.</summary>
        /// <param name="key">Key for identification</param>
        /// <param name="context">ContextName for sv</param>
        /// <param name="id">ID needed for svCache.GetSV, can be the returnvalue</param>
        /// <returns>The result of svCache.GetSV</returns>
        /// <exception cref="InternalError">Thrown when no application has been found in application scope.</exception>
        public static Constant get(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "XDoc.get").Value;
            var context = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "XDoc.get").Value;
            var id = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 2, "XDoc.get").Value;

            var svCache = Tool.GetFromScope<XHTMLMerge.SVCache>(scope, "__svCache__");
            if (svCache == null) throw new InternalError("No svCache");
            var value = svCache == null ? "" : svCache.GetSV(key, context + "_" + id, "");

            if (value.Length == 0) return Core.Javascript.Static.Undefined;
            return new Core.Javascript.String(value);
        }

        
        public static Constant set(Constant _this, Constant[] arguments, Scope scope) {
            var key = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "XDoc.get").Value;
            var context = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "XDoc.get").Value;
            var id = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 2, "XDoc.get").Value;
            var value = Core.Tool.GetArgument(arguments, 3, "Session.set");

            var svCache = Tool.GetFromScope<XHTMLMerge.SVCache>(scope, "__svCache__");
            if (svCache == null) throw new InternalError("No svCache");
            if (svCache != null) svCache.SetSV(key, context + "_" + id, value.ToString());

            return Static.Undefined;
        }
    }
}