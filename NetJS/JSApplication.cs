using System;
using NetJS.Core;
using System.Web;

namespace NetJS {
    public class JSApplication {

        public Cache Cache { get; }
        public Watch Watch { get; }
        public Connections Connections { get; }
        public External.Config Config { get; }
        public Settings Settings { get; }

        public Engine Engine { get; }

        public XDocServices.XDocService XDocService { get; }

        public JSApplication(string rootDir = null) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Settings = new Settings(rootDir);

            Watch = new Watch();
            Cache = new Cache();

            Engine = new Engine();
            Engine.Init();
            Engine.RegisterClass(typeof(External.HTTP));
            Engine.RegisterClass(typeof(External.SQL));
            Engine.RegisterClass(typeof(External.IO));
            Engine.RegisterClass(typeof(External.Log));
            Engine.RegisterClass(typeof(External.Session));
            Engine.RegisterClass(typeof(External.XDoc));
            Engine.RegisterClass(typeof(External.Base64));
            Engine.RegisterClass(typeof(External.Buffer));
            Engine.RegisterFunctions(typeof(External.Functions));

            Connections = new Connections(Watch, Settings);
            Config = new External.Config(Watch, Engine.Scope, Settings);

            XDocService = new XDocServices.XDocService();
        }

        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }
    }
}