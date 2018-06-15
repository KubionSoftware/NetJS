using System;
using System.Collections.Generic;
using NetJS.Core;

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
            public ScriptRecord Script;
            public SourceType Type;
        }

        private Dictionary<string, SourceFile> Files = new Dictionary<string, SourceFile>();

        public string GetPath(string name, JSApplication application, bool inSource = true) {
            var path = name;
            if (!System.IO.Path.IsPathRooted(path)) {
                path = application.Settings.Root + (inSource ? application.Settings.TemplateFolder : "") + path;
            }
            return path;
        }

        public ScriptRecord GetScript(string name, JSApplication application) {
            var path = GetPath(name, application);
            var key = Core.Tool.NormalizePath(path);

            if (Files.ContainsKey(key)) {
                var modified = System.IO.File.GetLastWriteTime(path);
                if (modified > Files[key].LastModified) {
                    Files[key] = LoadFile(path, application.Realm);
                }

                return Files[key].Script;
            } else {
                var sourceFile = LoadFile(path, application.Realm);
                Files[key] = sourceFile;
                return sourceFile.Script;
            }
        }

        private SourceFile LoadFile(string path, Realm realm) {
            string source;
            DateTime lastModified;

            try {
                source = System.IO.File.ReadAllText(path);
                lastModified = System.IO.File.GetLastWriteTime(path);
            }catch(Exception e) {
                throw new IOError($"Could not find file '{path}'");
            }

            var fileId = Core.Debug.GetFileId(path);

            if (path.EndsWith(".js")) {
                return new SourceFile() {
                    LastModified = lastModified,
                    Script = ScriptRecord.ParseScript(source, realm, fileId),
                    Type = SourceType.Javascript
                };
            } else {
                return new SourceFile() {
                    LastModified = lastModified,
                    Script = ScriptRecord.ParseTemplate(source, realm, fileId),
                    Type = SourceType.Javascript
                };
            }
        }
    }
}