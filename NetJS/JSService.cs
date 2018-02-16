using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NetJS { 
    public class JSService {

        public JSService() {

        }

        public string RunTemplate(string template) {
            return RunTemplate(template, "{}");
        }

        public string RunTemplate(string template, string data) {
            var application = new JSApplication();
            var session = new JSSession();
            var svCache = new XHTMLMerge.SVCache();

            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref XHTMLMerge.SVCache svCache) {
            var application = new JSApplication();
            var session = new JSSession();

            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session) {
            var svCache = new XHTMLMerge.SVCache();
            return RunTemplate(template, data, ref application, ref session, ref svCache);
        }

        public string RunTemplate(string template, string data, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            var arguments = Internal.JSON.parse(null, new[] { new Javascript.String(data) }, application.Global.Scope);
            if (arguments is Javascript.Object) {
                return RunTemplate(template, (Javascript.Object)arguments, ref application, ref session, ref svCache);
            }
            return "invalid arguments (must be valid json)";
        }

        public string RunTemplate(string template, Javascript.Object arguments, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                if (arguments == null) {
                    arguments = Tool.Construct("Object", application.Global.Scope);
                }

                // TODO: better way to forward session
                var result = Internal.Functions.include(
                    Javascript.Static.Undefined,
                    new Javascript.Constant[] { new Javascript.String(template), arguments },
                    new Javascript.Scope(application.Global.Scope, null, session, svCache)
                );

                if (result is Javascript.String) {
                    return ((Javascript.String)result).Value;
                }
            } catch (Javascript.Error e) {
                return e.Message + "<br>" + string.Join("<br>", e.StackTrace.Select(loc => Debug.GetFileName(loc.FileId) + " (" + loc.LineNr + ")"));
            } catch (Exception e) {
                return "System error - " + e.ToString();
            }

            return "";
        }

        public string RunTemplate(string template, Javascript.Object arguments, ref JSApplication application) {
            if(application == null) {
                application = new JSApplication();
            }

            var session = new JSSession();
            var svCache = new XHTMLMerge.SVCache();

            return RunTemplate(template, arguments, ref application, ref session, ref svCache);
        }

        public void ProcessRequest(HttpContext context, JSApplication application, JSSession session) {
            XHTMLMerge.SVCache svCache = null;

            var path = Tool.GetPath(context.Request);

            var arguments = Tool.Construct("Object", application.Global.Scope);
            arguments.Set("request", Tool.CreateRequest(context, path, application.Global.Scope));

            var responseString = RunTemplate(application.Settings.Entry, arguments, ref application, ref session, ref svCache);

            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            context.Response.AppendHeader("Access-Control-Allow-Headers", "X-Requested-With");

            var buffer = Encoding.UTF8.GetBytes(responseString);
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public void ProcessRequest(HttpContext context) {
            JSApplication application = null;
            JSSession session = null;

            if (context.Application != null) application = (JSApplication)context.Application["JSApplication"];
            if (application == null) application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory + "res/");

            if (context.Session != null) session = (JSSession)context.Session["JSSession"];
            if (session == null) session = new JSSession();

            ProcessRequest(context, application, session);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }

        public void Handle(HttpContext context) {
            JSApplication application = null;
            if (context.Application != null) application = (JSApplication)context.Application["JSApplication"];
            if (application == null) application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory + "res/");

            var mainTemplate = application.Settings.Root + application.Settings.TemplateFolder + application.Settings.Entry;

            if (mainTemplate.EndsWith(".js")) {
                ProcessRequest(context);
            }else if (mainTemplate.EndsWith(".xdoc")) {
                application.XDocService.ProcessRequest(context);
            }

            if (context.Application != null) context.Application["JSApplication"] = application;
        }
    }
}
