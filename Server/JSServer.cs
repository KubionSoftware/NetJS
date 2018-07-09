using Microsoft.ClearScript;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server {
    public class JSServer {

        private JSService _service;
        private JSApplication _application;
        public JSApplication Application { get { return _application; } }

        private ConcurrentDictionary<string, JSSession> _sessions;

        public JSServer() : this(new JSService()) { }

        public JSServer(JSService service) {
            _service = service;
            _application = CreateApplication();

            _sessions = new ConcurrentDictionary<string, JSSession>();
        }

        private JSApplication CreateApplication() {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory, (app) => {
                app.AddHostType(typeof(API.Request));
                app.AddHostType(typeof(API.Response));
                app.AddHostType(typeof(API.WebSocket));
                app.AddHostType(typeof(API.HTTPServer));

                var session = new JSSession();

                _service.RunCodeSync($"require('{app.Settings.Startup}')", app, session, (result) => { });
            }, (exception) => {
                string error = "";
                if (exception is ScriptEngineException se) {
                    error = se.ErrorDetails;
                } else {
                    error = exception.ToString();
                }

                try {
                    if (HttpContext.Current.Request.Url.Scheme.StartsWith("http")) {
                        API.Response.setHeader("Content-Type", "text/plain");
                        State.Request.ResultCallback(error);
                        return;
                    }
                } catch { }

                NetJS.API.Log.write(error);
            });

            return application;
        }

        public JSSession GetSession(HttpContext context) {
            var id = context.Session.SessionID;
            return _sessions.GetOrAdd(id, sessionId => {
                return new JSSession();
            });
        }

        public void ProcessRequest(HttpContext context, NetJS.JSApplication application, NetJS.JSSession session, Action after) {
            API.HTTPServer.OnConnection(application, session, after);
        }

        public void ProcessRequest(HttpContext context, Action after) {
            var application = _application;
            var session = GetSession(context);

            ProcessRequest(context, application, session, after);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }
    }
}