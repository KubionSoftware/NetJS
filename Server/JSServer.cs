using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace NetJS.Server {
    public class JSServer {

        private NetJS.JSService _service;

        public JSServer() : this(new JSService()) { }

        public JSServer(JSService service) {
            _service = service;
        }

        public JSApplication CreateApplication() {
            var application = new JSApplication(AppDomain.CurrentDomain.BaseDirectory);

            application.Engine.RegisterClass(typeof(API.Request));
            application.Engine.RegisterClass(typeof(API.Response));

            var session = new JSSession();
            _service.RunTemplate(application.Settings.Startup, "", ref application, ref session);

            return application;
        }

        public void ProcessRequest(HttpContext context, NetJS.JSApplication application, NetJS.JSSession session) {
            var responseString = _service.RunTemplate(application.Settings.Entry, "", ref application, ref session);

            var buffer = Encoding.UTF8.GetBytes(responseString);
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public void ProcessRequest(HttpContext context) {
            NetJS.JSApplication application = null;
            NetJS.JSSession session = null;

            if (context.Application != null) application = (NetJS.JSApplication)context.Application["JSApplication"];
            if (application == null) application = CreateApplication();

            if (context.Session != null) session = (NetJS.JSSession)context.Session["JSSession"];
            if (session == null) session = new NetJS.JSSession();

            ProcessRequest(context, application, session);

            if (context.Application != null) context.Application["JSApplication"] = application;
            if (context.Session != null) context.Session["JSSession"] = session;
        }

        public void Handle(HttpContext context) {
            JSApplication application = null;
            if (context.Application != null) application = (JSApplication)context.Application["JSApplication"];
            if (application == null) application = CreateApplication();

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