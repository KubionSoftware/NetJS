using System;
using System.Web;
using NetJS.Core;
using Microsoft.ClearScript.V8;

namespace NetJS {
    public class JSApplication : JSStorage {

        public Cache Cache { get; }
        public Connections Connections { get; }
        public API.Config Config { get; }
        public Settings Settings { get; }

        public Realm Realm { get; }

        public V8ScriptEngine V8;

        public XDocServices.XDocService XDocService { get; }

        public JSApplication(string rootDir = null) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            V8 = new V8ScriptEngine();

            Settings = new Settings(rootDir);

            Cache = new Cache();
            
            Realm = new Realm();
            Realm.SetAgent(new NetJSAgent(Realm, this, new JSSession()));

            Realm.RegisterClass(typeof(API.HTTP), "HTTP");
            Realm.RegisterClass(typeof(API.SQL), "SQL");
            Realm.RegisterClass(typeof(API.IO), "IO");
            Realm.RegisterClass(typeof(API.Log), "Log");
            Realm.RegisterClass(typeof(API.Session), "Session");
            Realm.RegisterClass(typeof(API.XDoc), "XDoc");
            Realm.RegisterClass(typeof(API.Base64), "Base64");
            Realm.RegisterClass(typeof(API.Buffer), "Buffer");
            Realm.RegisterClass(typeof(API.Windows), "Windows");
            Realm.RegisterClass(typeof(API.Async), "Async");
            Realm.RegisterClass(typeof(API.DLL), "DLL");
            Realm.RegisterClass(typeof(API.XML), "XML");
            Realm.RegisterFunctions(typeof(API.Functions));

            Connections = new Connections(Settings);
            Config = new API.Config(Realm.GetAgent(), Settings);

            XDocService = new XDocServices.XDocService();
        }

        public void ProcessXDocRequest(HttpContext context) {
            XDocService.ProcessRequest(context);
        }
    }
}