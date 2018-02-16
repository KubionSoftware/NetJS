using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS {
    public class JSApplication {

        public Global Global { get; }
        public Watch Watch { get; }
        public Connections Connections { get; }
        public External.Config Config { get; }
        public Settings Settings { get; }

        public XDocServices.XDocService4 XDocService { get; }

        public JSApplication(string rootDir = null) {
            if (rootDir == null) {
                rootDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            Settings = new Settings(rootDir);

            Watch = new Watch();
            Global = new Global(this);
            Global.Init();

            Connections = new Connections(Watch, Settings);
            Config = new External.Config(Watch, Global.Scope, Settings);

            XDocService = new XDocServices.XDocService4();
        }
    }
}