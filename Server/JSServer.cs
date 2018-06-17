using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server {
    public class JSServer {

        private JSService _service;
        private JSApplication _application;

        public JSApplication Application { get { return _application; } }

        public JSServer() : this(new JSService()) { }

        public JSServer(JSService service) {
            _service = service;
            _application = CreateApplication();
        }

        public JSApplication CreateApplication() {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory);

            application.Realm.RegisterClass(typeof(API.Request), "Request");
            application.Realm.RegisterClass(typeof(API.Response), "Response");
            application.Realm.RegisterClass(typeof(API.WebSocket), "WebSocket");

            var session = new JSSession();
            _service.RunTemplate(application.Settings.Startup, "", ref application, ref session);

            return application;
        }

        public JSSession GetSession(HttpContext context) {
            NetJS.JSSession session = null;

            if (context.Session != null) session = (NetJS.JSSession)context.Session["JSSession"];
            if (session == null) session = new NetJS.JSSession();

            return session;
        }

        public void ProcessRequest(HttpContext context, NetJS.JSApplication application, NetJS.JSSession session) {
            var responseString = _service.RunTemplate(application.Settings.Entry, "", ref application, ref session);

            var buffer = Encoding.UTF8.GetBytes(responseString);
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public void ProcessRequest(HttpContext context) {
            var application = _application;
            var session = GetSession(context);

            ProcessRequest(context, application, session);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }

        public void Handle(HttpContext context) {
            var application = _application;
            var mainTemplate = application.Settings.Root + application.Settings.TemplateFolder + application.Settings.Entry;

            if (mainTemplate.EndsWith(".js")) {
                ProcessRequest(context);
            } else if (mainTemplate.EndsWith(".xdoc")) {
                application.ProcessXDocRequest(context);
            }

            if (context.Application != null) context.Application["JSApplication"] = application;
        }
    }
}