using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    public class XDoc {

        public static Javascript.Constant include(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "XDoc.include");

            var context = HttpContext.Current;

            XHTMLMerge.AppCache appCache = null;
            XHTMLMerge.SVCache svCache = null;

            if (context != null && context.Application != null) appCache = (XHTMLMerge.AppCache)context.Application["AppCache"];
            if (appCache == null) appCache = new XHTMLMerge.AppCache();

            if (context != null && context.Session != null) svCache = (XHTMLMerge.SVCache)context.Session["SVCache"];
            if (svCache == null) svCache = new XHTMLMerge.SVCache();

            var parameters = new Hashtable();
            if (arguments.Length > 1) {
                var param = (Javascript.Object)arguments[1];
                foreach (var key in param.GetKeys()) {
                    parameters.Add(key, param.Get(key).ToString());
                }
            }
            
            var result = scope.Application.XDocService.RunTemplate(context, name.Value, parameters, ref appCache, ref svCache);

            if (context != null && context.Application != null) context.Application["AppCache"] = appCache;
            if (context != null && context.Session != null) context.Session["SVCache"] = svCache;

            return new Javascript.String(result);
        }

        public static Javascript.Constant get(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var key = Tool.GetArgument<Javascript.String>(arguments, 0, "XDoc.get").Value;
            var context = Tool.GetArgument<Javascript.String>(arguments, 1, "XDoc.get").Value;
            var id = Tool.GetArgument<Javascript.String>(arguments, 2, "XDoc.get").Value;

            var value = scope.SVCache == null ? "" : scope.SVCache.GetSV(key, context + "_" + id, "");

            if (value.Length == 0) return Javascript.Static.Undefined;
            return new Javascript.String(value);
        }

        public static Javascript.Constant set(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var key = Tool.GetArgument<Javascript.String>(arguments, 0, "XDoc.get").Value;
            var context = Tool.GetArgument<Javascript.String>(arguments, 1, "XDoc.get").Value;
            var id = Tool.GetArgument<Javascript.String>(arguments, 2, "XDoc.get").Value;
            var value = Tool.GetArgument(arguments, 3, "Session.set");

            if (scope.SVCache != null) scope.SVCache.SetSV(key, context + "_" + id, value.ToString());

            return Javascript.Static.Undefined;
        }
    }
}