using System;
using NetJS.Core;
using System.Web;

namespace NetJS {
    public class JSApplication : JSStorage {

        public Cache Cache { get; }
        public Connections Connections { get; }
        public API.Config Config { get; }
        public Settings Settings { get; }

        public Engine Engine { get; }

        public XDocServices.XDocService XDocService { get; }

        public JSApplication(string rootDir = null) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Settings = new Settings(rootDir);
            
            Cache = new Cache();

            Engine = new Engine();
            Engine.Init();
            Engine.RegisterClass(typeof(API.HTTP));
            Engine.RegisterClass(typeof(API.SQL));
            Engine.RegisterClass(typeof(API.IO));
            Engine.RegisterClass(typeof(API.Log));
            Engine.RegisterClass(typeof(API.Session));
            Engine.RegisterClass(typeof(API.XDoc));
            Engine.RegisterClass(typeof(API.Base64));
            Engine.RegisterClass(typeof(API.Buffer));
            Engine.RegisterClass(typeof(API.Windows));
            Engine.RegisterClass(typeof(API.Async));
            Engine.RegisterClass(typeof(API.DLL));
            Engine.RegisterClass(typeof(API.XML));
            Engine.RegisterFunctions(typeof(API.Functions));

            Connections = new Connections(Settings);
            Config = new API.Config(Engine.GlobalScope, Settings);

            XDocService = new XDocServices.XDocService();
        }

        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }
    }
}