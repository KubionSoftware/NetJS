using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NetJS {
    public class Settings {

        public string Root { get; }

        public string TemplateFolder { get; } = "src/";
        public string Entry { get; } = "main.js";
        public string Config { get; } = "config.json";
        public string Connections { get; } = "connections.json";
        public string Log { get; } = "log.txt";

        public Settings(string root) {
            Root = GetValue("JSFiles", root);
            TemplateFolder = GetValue("JSTemplates", TemplateFolder);
            Entry = GetValue("JSEntry", Entry);
            Config = GetValue("JSConfig", Config);
            Connections = GetValue("JSConnections", Connections);
            Log = GetValue("JSLog", Log);

            if (!Root.EndsWith("/")) Root += "/";
            if (!TemplateFolder.EndsWith("/")) TemplateFolder += "/";
        }

        public static string GetValue(string key, string def) {
            var value = ConfigurationManager.AppSettings[key];
            if(value != null) {
                return value.ToString();
            }
            return def;
        }
    }
}