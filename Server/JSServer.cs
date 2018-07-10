using Microsoft.ClearScript;
using System;
using System.Collections.Concurrent;
using System.Web;

namespace NetJS.Server {
    public class JSServer {

        private JSService _service;
        private readonly JSApplication _application;
        public JSApplication Application { get { return _application; } }

        private ConcurrentDictionary<string, JSSession> _sessions;

        public JSServer(Action after) : this(new JSService(), after) { }

        public string CompileError = "";

        public JSServer(JSService service, Action after) {
            _service = service;
            _application = CreateApplication(after);

            _sessions = new ConcurrentDictionary<string, JSSession>();
        }

        private JSApplication CreateApplication(Action after) {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory, (app) => {
                // Define the server API
                app.AddHostType(typeof(API.Request));
                app.AddHostType(typeof(API.Response));
                app.AddHostType(typeof(API.WebSocket));
                app.AddHostType(typeof(API.HTTPServer));

                var session = new JSSession();

                // Clear the compile error
                CompileError = "";

                // Run the startup file
                var require = app.Evaluate(@"(function(file){
                    require(file);
                }).valueOf()");
                var callback = API.HTTPServer.Callback(after);
                var request = new ServerRequest(require, app, callback, session, HttpContext.Current, app.Settings.Startup);
                request.Call();
            }, (exception, stage) => {
                // Get the error message
                string error = "";
                if (exception is ScriptEngineException se) {
                    error = se.ErrorDetails;
                } else {
                    error = exception.ToString();
                }

                // If it is a compilation error, store it in the variable "CompileError"
                if (stage == ErrorStage.Compilation) {
                    CompileError = error;
                }

                // Try to show the error message in the browser
                try {
                    API.Response.setHeader("Content-Type", "text/plain");
                    State.Request.ResultCallback(error);
                    return;
                } catch (Exception e) { }

                // If that failed, write it to the log
                NetJS.API.Log.write(error);
            });

            return application;
        }

        // Creates a new session with the id, or returns an existing session with that id
        public JSSession GetSession(string id) {
            return _sessions.GetOrAdd(id, sessionId => {
                return new JSSession();
            });
        }

        public JSSession GetSession(HttpContext context) {
            var id = context.Session.SessionID;
            return GetSession(id);
        }

        public void ProcessRequest(HttpContext context, Action after) {
            if (CompileError.Length > 0) {
                context.Response.ContentType = "text/plain";
                context.Response.Write(CompileError);
                context.Response.End();
                after();
                return;
            }
            
            var session = GetSession(context);

            API.HTTPServer.OnConnection(_application, session, after);
        }
    }
}