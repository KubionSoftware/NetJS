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

        public Block GetFile(string name, JSApplication application) {
            var path = name;
            if (!System.IO.Path.IsPathRooted(path)) {
                path = application.Settings.Root + application.Settings.TemplateFolder + path;
            }
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
            var source = System.IO.File.ReadAllText(path);
            var lastModified = System.IO.File.GetLastWriteTime(path);

            if (path.EndsWith(".js")) {
                var fileId = Core.Debug.GetFileId(path);
                var tokens = Lexer.Lex(source, fileId);
                var parser = new Parser(fileId, tokens);

                return new SourceFile() {
                    LastModified = lastModified,
                    Node = parser.Parse(),
                    Type = SourceType.Javascript
                };
            } else {
                var fileId = Core.Debug.GetFileId(path);
                File file = Parser.ParseFile(source, fileId);

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