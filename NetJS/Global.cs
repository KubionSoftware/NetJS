using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace NetJS {
    public class Global {

        private enum SourceType {
            None,
            Javascript,
            XDoc
        }

        private class SourceFile {
            public DateTime LastModified;
            public Javascript.Block Node;
            public SourceType Type;
        }

        public Javascript.Scope Scope { get; private set; }
        private Dictionary<string, Javascript.Object> _prototypes = new Dictionary<string, Javascript.Object>();

        private Dictionary<string, SourceFile> Files = new Dictionary<string, SourceFile>();

        public Javascript.Object GetPrototype(string name) {
            return _prototypes[name];
        }

        public Global(JSApplication application) {
            Scope = new Javascript.Scope(application, null, null);
        }

        public void Init() {
            var objectPrototype = new Javascript.Object(null);
            var functionPrototype = new Javascript.Object(objectPrototype);

            var objectConstructor = new Javascript.Object(functionPrototype);
            objectConstructor.Set("prototype", objectPrototype);
            objectPrototype.Set("constructor", objectConstructor);

            var functionConstructor = new Javascript.Object(functionPrototype);
            functionConstructor.Set("prototype", functionPrototype);
            functionPrototype.Set("constructor", functionConstructor);

            _prototypes["Object"] = objectConstructor;
            _prototypes["Function"] = functionConstructor;

            RegisterType(typeof(Internal.Array));
            RegisterType(typeof(Internal.Date));
            RegisterType(typeof(Internal.String));
            RegisterType(typeof(Internal.Number));
            RegisterType(typeof(Internal.Boolean));
            RegisterType(typeof(Internal.RegExp));

            RegisterClass(typeof(Internal.JSON));
            RegisterClass(typeof(Internal.Math));

            RegisterClass(typeof(External.HTTP));
            RegisterClass(typeof(External.DB));
            RegisterClass(typeof(External.IO));
            RegisterClass(typeof(External.Log));
            RegisterClass(typeof(External.Session));

            RegisterFunctions(typeof(Internal.Functions));
        }

        public Javascript.Block GetFile(string name, JSApplication application) {
            var path = application.Settings.Root + application.Settings.TemplateFolder + name;
            var key = Tool.NormalizePath(path);

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
                var tokens = Javascript.Lexer.Lex(source);
                var parser = new Javascript.Parser(path, tokens);

                return new SourceFile() {
                    LastModified = lastModified,
                    Node = parser.Parse(),
                    Type = SourceType.Javascript
                };
            }

            return new SourceFile() {
                LastModified = lastModified,
                Node = new Javascript.Block(),
                Type = SourceType.None
            };
        }

        private Javascript.ExternalFunction GetFunction(MethodInfo info) {
            return new Javascript.ExternalFunction(
                (Func<
                    Javascript.Constant, 
                    Javascript.Constant[], 
                    Javascript.Scope, 
                    Javascript.Constant
                >)info.CreateDelegate(typeof(Func<
                    Javascript.Constant, 
                    Javascript.Constant[], 
                    Javascript.Scope, 
                    Javascript.Constant
                >)), Scope);
        }

        private void RegisterClass(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var obj = Tool.Construct("Object", Scope);

            foreach (var method in methods) {
                obj.Set(method.Name, GetFunction(method));
            }

            new Javascript.Variable(type.Name).Assignment(obj, Scope);
        }

        private void RegisterType(Type type) {
            var prototype = Tool.Construct("Object", Scope);

            var constructor = GetFunction(type.GetMethod("constructor"));
            constructor.Set("prototype", prototype);
            prototype.Set("constructor", constructor);

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                if (method.Name != "constructor") {
                    prototype.Set(method.Name, GetFunction(method));
                }
            }

            Scope.SetVariable(type.Name, constructor);
            _prototypes[type.Name] = constructor;
        }

        public void RegisterFunctions(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                Scope.SetVariable(method.Name, GetFunction(method));
            }
        }
    }
}