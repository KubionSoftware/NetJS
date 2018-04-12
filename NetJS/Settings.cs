using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NetJS {
    public class Settings {

        public string Root { get; private set; }

        public string TemplateFolder { get; private set; } = "src/";
        public string Entry { get; private set; } = "main.js";
        public string Config { get; private set; } = "config.json";
        public string Connections { get; private set; } = "connections.json";
        public string Log { get; private set; } = "log.txt";

        public Settings(string root) {
            SetRoot(root);
        }

        public void Set(string key, string value) {
            if (key == "JSFiles") SetRoot(value);
            if (key == "JSTemplates") SetTemplateFolder(value);
            if (key == "JSEntry") Entry = value;
            if (key == "JSConfig") Config = value;
            if (key == "JSConnections") Connections = value;
            if (key == "JSLog") Log = value;
        }

        public void SetRoot(string root) {
            if (!root.EndsWith("/")) root += "/";
            Root = root;
        }

        public void SetTemplateFolder(string templateFolder) {
            if (!templateFolder.EndsWith("/")) templateFolder += "/";
            TemplateFolder = templateFolder;
        }
    }
}