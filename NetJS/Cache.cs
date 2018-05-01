using System;
using System.Collections.Generic;
using NetJS.Core.Javascript;

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
            public Block Node;
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

        public Block GetFile(string name, JSApplication application) {
            var path = GetPath(name, application);
            var key = Core.Tool.NormalizePath(path);

            if (Files.ContainsKey(key)) {
                var modified = System.IO.File.GetLastWriteTime(path);
                if (modified > Files[key].LastModified) {
                    Files[key] = LoadFile(path);
                }

                return Files[key].Node;
            } else {
                var sourceFile = LoadFile(path);
                Files[key] = sourceFile;
                return sourceFile.Node;
            }
        }

        private SourceFile LoadFile(string path) {
            string source;
            DateTime lastModified;

            try {
                source = System.IO.File.ReadAllText(path);
                lastModified = System.IO.File.GetLastWriteTime(path);
            }catch(Exception e) {
                throw new IOError($"Could not find file '{path}'");
            }

            if (path.EndsWith(".js") || path.EndsWith(".ts")) {
                var fileId = Core.Debug.GetFileId(path);
                var tokens = new Lexer(source, fileId).Lex();
                var parser = new Parser(fileId, tokens);

                return new SourceFile() {
                    LastModified = lastModified,
                    Node = parser.Parse(),
                    Type = SourceType.Javascript
                };
            } else {
                var fileId = Core.Debug.GetFileId(path);
                var tokens = new Lexer("`" + source + "`", fileId).Lex();
                var parser = new Parser(fileId, tokens);
                File file = parser.ParseFile();

                return new SourceFile() {
                    LastModified = lastModified,
                    Node = new Block() {
                        Nodes = new List<Node>() { file }
                    },
                    Type = SourceType.Other
                };
            }
        }
    }
}