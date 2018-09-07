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

        public JSServer(HttpContext context, Action after) : this(new JSService(), context, after) { }

        public string CompileError = "";

        public JSServer(JSService service, HttpContext context, Action after) {
            _service = service;
            _application = CreateApplication(context, after);

            _sessions = new ConcurrentDictionary<string, JSSession>();
        }

        private JSApplication CreateApplication(HttpContext context, Action after) {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory, (app) => {
                // Reset the websocket and http hooks
                API.WebSocket.ResetHooks();
                API.HTTPServer.ResetHooks();

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
                var callback = API.HTTPServer.Callback(after, context);
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

                WriteError(context, after, error);
            });

            return application;
        }

        private static void WriteError(HttpContext context, Action after, string error) {
            try {
                NetJS.API.Log.write(error);
            } catch (Exception e) { }

            // Try with the context in state
            try {
                API.Response.setHeader("Content-Type", "text/plain");
                NetJS.API.Functions.end(error);
                return;
            } catch (Exception e) { }

            // Try with the given context
            try {
                Tool.End(context, error);
                after();
                return;
            } catch (Exception e) { }
            
            after();
        }

        // Creates a new session with the id, or returns an existing session with that id
        public JSSession GetSession(string id) {
            return _sessions.GetOrAdd(id, sessionId => {
                return new JSSession();
            });
        }

        public JSSession GetSession(HttpContext context) {
            var id = context.Session != null ? context.Session.SessionID : Guid.NewGuid().ToString();
            return GetSession(id);
        }

        public void ProcessRequest(HttpContext context, Action after) {
            if (CompileError.Length > 0) {
                WriteError(context, after, CompileError);
                return;
            }
            
            var session = GetSession(context);

            API.HTTPServer.OnConnection(_application, session, after);
        }
    }
}