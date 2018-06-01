using System;
using NetJS.Core;
using System.Web;
using Microsoft.ClearScript.V8;

namespace NetJS {
    public class JSApplication : JSStorage {

        public Cache Cache { get; }
        public Connections Connections { get; }
        public API.Config Config { get; }
        public Settings Settings { get; }

        public V8ScriptEngine Engine { get; }

        public XDocServices.XDocService XDocService { get; }

        public JSApplication(string rootDir = null) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Settings = new Settings(rootDir);
            
            Cache = new Cache();

            Engine = new V8ScriptEngine();

            Engine.AddHostType(typeof(API.HTTP));
            Engine.AddHostType(typeof(API.SQL));
            Engine.AddHostType(typeof(API.IO));
            Engine.AddHostType(typeof(API.Log));
            Engine.AddHostType(typeof(API.Session));
            Engine.AddHostType(typeof(API.XDoc));
            Engine.AddHostType(typeof(API.Base64));
            Engine.AddHostType(typeof(API.Buffer));
            Engine.AddHostType(typeof(API.Windows));
            Engine.AddHostType(typeof(API.Async));
            Engine.AddHostType(typeof(API.DLL));
            Engine.AddHostType(typeof(API.XML));
            Engine.AddHostType(Microsoft.ClearScript.HostItemFlags.GlobalMembers, typeof(API.Functions));

            Connections = new Connections(Settings);

            Config = new API.Config(Settings);
            Engine.AddHostObject("Config", Config);

            XDocService = new XDocServices.XDocService();
        }

        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }
    }
}