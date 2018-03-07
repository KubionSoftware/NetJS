using System;
using System.Linq;
using System.Text;
using System.Web;
using NetJS.Core.Javascript;
using NetJS.Core.Internal;
using NetJS.Core;

namespace NetJS { 
    public class JSService {

        public JSService() {

        }

        public static string[] GetPath(HttpRequest request) {
            return request.Url.PathAndQuery.Split('?')[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Core.Javascript.Object CreateRequest(HttpContext context, string[] path, Scope scope) {
            var request = Core.Tool.Construct("Object", scope);
            request.Set("path", Core.Tool.ToArray(path, scope));

            var form = Core.Tool.Construct("Object", scope);
            for (var i = 0; i < context.Request.Form.Count; i++) {
                var key = context.Request.Form.GetKey(i);
                var value = context.Request.Form.Get(i);
                form.Set(key, new Core.Javascript.String(value));
            }
            request.Set("form", form);

            var parameters = Core.Tool.Construct("Object", scope);
            var query = HttpUtility.ParseQueryString(context.Request.Url.Query);
            for (var i = 0; i < query.Count; i++) {
                var key = query.GetKey(i);
                if (key == null) continue;

                var value = query.Get(i);
                parameters.Set(key, new Core.Javascript.String(value));
            }
            request.Set("params", parameters);

            var content = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
            request.Set("content", new Core.Javascript.String(content));

            request.Set("method", new Core.Javascript.String(context.Request.HttpMethod));
            request.Set("url", new Core.Javascript.String(context.Request.RawUrl));
            request.Set("scheme", new Core.Javascript.String(context.Request.Url.Scheme));

            return request;
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
            var arguments = JSON.parse(null, new[] { new Core.Javascript.String(data) }, application.Engine.Scope);
            if (arguments is Core.Javascript.Object a) {
                return RunTemplate(template, a, ref application, ref session, ref svCache);
            }
            return "invalid arguments (must be valid json)";
        }

        // Every template is executed via this function
        public string RunTemplate(string template, Core.Javascript.Object arguments, ref JSApplication application, ref JSSession session, ref XHTMLMerge.SVCache svCache) {
            try {
                if (arguments == null) {
                    arguments = Core.Tool.Construct("Object", application.Engine.Scope);
                }

                var scope = new Scope(application.Engine.Scope, null, ScopeType.Session);
                scope.SetVariable("__application__", new Foreign(application));
                scope.SetVariable("__session__", new Foreign(session));
                scope.SetVariable("__svCache__", new Foreign(svCache));

                // TODO: better way to forward session
                var result = External.Functions.include(
                    Static.Undefined,
                    new Constant[] { new Core.Javascript.String(template), arguments },
                    scope
                );

                if (result is Core.Javascript.String s) {
                    return s.Value;
                }
            } catch (Error e) {
                return e.Message + "\n" + string.Join("\n", e.StackTrace.Select(loc => Debug.GetFileName(loc.FileId) + " (" + loc.LineNr + ")"));
            } catch (Exception e) {
                return "System error - " + e.ToString();
            }

            return "";
        }

        public string RunTemplate(string template, Core.Javascript.Object arguments, ref JSApplication application) {
            if(application == null) {
                application = new JSApplication();
            }

            var session = new JSSession();
            var svCache = new XHTMLMerge.SVCache();

            return RunTemplate(template, arguments, ref application, ref session, ref svCache);
        }

        public void ProcessRequest(HttpContext context, JSApplication application, JSSession session) {
            XHTMLMerge.SVCache svCache = null;

            var path = GetPath(context.Request);

            var arguments = Core.Tool.Construct("Object", application.Engine.Scope);
            arguments.Set("request", CreateRequest(context, path, application.Engine.Scope));

            var response = Core.Tool.Construct("Object", application.Engine.Scope);
            response.Set("contentType", new Core.Javascript.String("text/html"));
            response.Set("statusCode", new Core.Javascript.Number(200));
            arguments.Set("response", response);

            var responseString = RunTemplate(application.Settings.Entry, arguments, ref application, ref session, ref svCache);

            try {
                context.Response.ContentType = response.Get<Core.Javascript.String>("contentType").Value;
                context.Response.StatusCode = (int)response.Get<Core.Javascript.Number>("statusCode").Value;
            }catch(Exception) {
                // TODO: log error
            }

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
            if (application == null) application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory);

            if (context.Session != null) session = (JSSession)context.Session["JSSession"];
            if (session == null) session = new JSSession();

            ProcessRequest(context, application, session);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }

        public void Handle(HttpContext context) {
            JSApplication application = null;
            if (context.Application != null) application = (JSApplication)context.Application["JSApplication"];
            if (application == null) application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory);

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
