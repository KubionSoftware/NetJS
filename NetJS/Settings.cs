using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NetJS {
    public class Settings {

        public string Root;

        public string TemplateFolder = "src/";
        public string Entry = "main.js";
        public string Config = "config.json";
        public string Connections = "connections.json";
        public string Log = "log.txt";

        public Settings(string root) {
            Root = root;

            TemplateFolder = GetValue("JSTemplates", TemplateFolder);
            Entry = GetValue("JSEntry", Entry);
            Config = GetValue("JSConfig", Config);
            Connections = GetValue("JSConnections", Connections);
            Log = GetValue("JSLog", Log);
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