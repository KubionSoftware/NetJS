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
            return State.Application.GetCurrentLocation();
        }

        public V8Script GetScript(string name, JSApplication application) {
            var path = GetPath(name, application, true);

            try { 
                var source = System.IO.File.ReadAllText(path);
                return application.Compile(path, source);
            } catch (System.IO.IOException) {
                application.Error(new IOError($"Could not load file '{path}'"), ErrorStage.Compilation);
            } catch (Exception e) {
                application.Error(e, ErrorStage.Compilation);
            }

            // Return an empty script to not block rest of execution
            return application.Compile(path, "");
        }

        public dynamic GetResource(string name, bool returnVar, JSApplication application) {
            var path = GetPath(name, application, true);

            try {
                var source = System.IO.File.ReadAllText(path);
                var code = Transpiler.TranspileTemplate(source, returnVar);
                var script = application.Compile(path, code);
                var function = application.Evaluate(script);
                return function;
            } catch (System.IO.IOException) {
                application.Error(new IOError($"Could not load file '{path}'"), ErrorStage.Compilation);
            } catch (Exception e) {
                application.Error(e, ErrorStage.Compilation);
            }

            // Return an empty function to not block rest of execution
            var c = Transpiler.TranspileTemplate("", returnVar);
            var s = application.Compile(name, c);
            var f = application.Evaluate(s);
            return f;
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