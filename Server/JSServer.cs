using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server {
    public class JSServer {

        private JSService _service;
        public readonly JSApplication Application;

        public JSServer() : this(new JSService()) { }

        public JSServer(JSService service) {
            _service = service;
            Application = CreateApplication();
        }

        public JSApplication CreateApplication() {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory, (app) => {
                app.AddHostType(typeof(API.Request));
                app.AddHostType(typeof(API.Response));
                app.AddHostType(typeof(API.WebSocket));
                app.AddHostType(typeof(API.HTTPServer));

                var session = new JSSession();

                _service.RunCodeSync($"require('{app.Settings.Startup}')", app, session, (result) => { });
            }, (exception) => {
                API.Response.setHeader("Content-Type", "text/plain");

                if (exception is ScriptEngineException se) {
                    State.Request.ResultCallback(se.ErrorDetails);
                } else {
                    State.Request.ResultCallback(exception.ToString());
                }
            });

            return application;
        }

        public JSSession GetSession(HttpContext context) {
            NetJS.JSSession session = null;

            if (context.Session != null) session = (NetJS.JSSession)context.Session["JSSession"];
            if (session == null) session = new NetJS.JSSession();

            return session;
        }

        public void ProcessRequest(HttpContext context, NetJS.JSApplication application, NetJS.JSSession session, Action after) {
            API.HTTPServer.OnConnection(application, session, after);
        }

        public void ProcessRequest(HttpContext context, Action after) {
            var application = Application;
            var session = GetSession(context);

            ProcessRequest(context, application, session, after);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }

        public void Handle(HttpContext context, Action after) {
            var application = Application;
            var mainTemplate = application.Settings.Root + application.Settings.TemplateFolder + application.Settings.Entry;

            if (mainTemplate.EndsWith(".js")) {
                ProcessRequest(context, after);
            } else if (mainTemplate.EndsWith(".xdoc")) {
                application.ProcessXDocRequest(context);
            }

            if (context.Application != null) context.Application["JSApplication"] = application;
        }
    }
}