using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetJS {
    public class Cache {

        private enum SourceType {
            None,
            Javascript,
            XDoc,
            Other
        }

        private class SourceFile {
            public DateTime LastModified;
            public V8Script Script;
            public SourceType Type;
        }

        private Dictionary<string, SourceFile> Files = new Dictionary<string, SourceFile>();

        public string GetPath(string name, JSApplication application, bool inSource) {
            var path = name;
            if (path.StartsWith("/")) {
                path = application.Settings.Root + (inSource ? application.Settings.TemplateFolder : "") + path;
            } else if (!System.IO.Path.IsPathRooted(path)) {
                var currentLocation = GetCurrentLocation();
                var nameParts = currentLocation.Split('/');
                path = application.Settings.Root + (inSource ? application.Settings.TemplateFolder : "");
                for (var i = 0; i < nameParts.Length - 1; i++) path += nameParts[i] + "/";
                path += name;
            }
            return path;
        }

        public string NormalizePath(string path) {
            return System.IO.Path.GetFullPath(path)
               .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
               .ToUpperInvariant();
        }

        public string GetCurrentLocation() {
            if (State.Application == null) return "";
            return State.Application.GetCurrentLocation().Replace("-", "/");
        }

        public V8Script GetScript(string name, JSApplication application) {
            var path = GetPath(name, application, true);
            var source = System.IO.File.ReadAllText(path);
            return application.Compile(path, source);
        }

        public dynamic GetResource(string name, bool returnVar, JSApplication application) {
            var path = GetPath(name, application, true);
            var source = "";

            try {
                source = System.IO.File.ReadAllText(path);
            }catch(Exception e) {
                application.Error(e);
            }

            var code = Transpiler.TranspileTemplate(source, returnVar);
            var script = application.Compile(name.Replace("/", "-"), code);
            var function = application.Evaluate(script);
            return function;
        }

        /*
        public V8Script GetScript(string name, JSApplication application) {
            var path = GetPath(name, application, true);
            var key = NormalizePath(path);

            if (Files.ContainsKey(key)) {
                var modified = System.IO.File.GetLastWriteTime(path);
                if (modified > Files[key].LastModified) {
                    Files[key] = LoadFile(path, application);
                }

                return Files[key].Script;
            } else {
                var sourceFile = LoadFile(path, application);
                Files[key] = sourceFile;
                return sourceFile.Script;
            }
        }

        private SourceFile LoadFile(string path, JSApplication application) {
            string source;
            DateTime lastModified;

            try {
                source = System.IO.File.ReadAllText(path);
                lastModified = System.IO.File.GetLastWriteTime(path);
            }catch(Exception e) {
                throw new IOError($"Could not find file '{path}'");
            }

            if (path.EndsWith(".js")) {
                return new SourceFile() {
                    LastModified = lastModified,
                    Script = application.Compile(path, source),
                    Type = SourceType.Javascript
                };
            } else {
                return new SourceFile() {
                    LastModified = lastModified,
                    Script = application.Compile(path, TranspileTemplate(source)),
                    Type = SourceType.Other
                };
            }
        }
        */
    }
}