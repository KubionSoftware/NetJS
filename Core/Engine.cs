using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetJS.Core {
    public class Engine {
        
        public Javascript.Scope Scope { get; private set; }
        private Dictionary<string, Javascript.Object> _prototypes = new Dictionary<string, Javascript.Object>();

        public Javascript.Object GetPrototype(string name) {
            if (!_prototypes.ContainsKey(name)) {
                throw new Javascript.InternalError($"Could not find prototype '{name}'");
            }
            return _prototypes[name];
        }

        public Engine() {
            Scope = new Javascript.Scope(this, null);
        }

        public void Init() {
            // Object and Function need to be bootstrapped because they are dependent on each other
            var objectPrototype = new Javascript.Object(null);
            var functionPrototype = new Javascript.Object(objectPrototype);

            var objectConstructor = new Javascript.Object(functionPrototype);
            objectConstructor.Set("prototype", objectPrototype);
            objectPrototype.Set("constructor", objectConstructor);

            var functionConstructor = new Javascript.Object(functionPrototype);
            functionConstructor.Set("prototype", functionPrototype);
            functionPrototype.Set("constructor", functionConstructor);

            Scope.DeclareVariable("Object", Javascript.DeclarationScope.Global, true, objectConstructor);
            _prototypes["Object"] = objectConstructor;

            Scope.DeclareVariable("Function", Javascript.DeclarationScope.Global, true, functionConstructor);
            _prototypes["Function"] = functionConstructor;

            RegisterType(typeof(API.Object));
            RegisterType(typeof(API.Function));
            RegisterType(typeof(API.Array));
            RegisterType(typeof(API.Uint8Array));
            RegisterType(typeof(API.Date));
            RegisterType(typeof(API.String));
            RegisterType(typeof(API.Number));
            RegisterType(typeof(API.Boolean));
            RegisterType(typeof(API.RegExp));

            RegisterClass(typeof(API.JSON));
            RegisterClass(typeof(API.Math));

            RegisterFunctions(typeof(API.Functions));
        }

        public void RegisterDLL(string file) {
            var dll = Assembly.LoadFile(file);

            foreach (Type type in dll.GetExportedTypes()) {
                RegisterClass(type);
            }
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

        public void RegisterClass(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var obj = Tool.Construct("Object", Scope);

            foreach (var method in methods) {
                try {
                    obj.Set(method.Name.Replace("@", ""), GetFunction(method));
                } catch { }
            }

            Scope.DeclareVariable(type.Name, Javascript.DeclarationScope.Global, true, obj);
        }

        public void RegisterType(Type type) {
            var prototype = Tool.Construct("Object", Scope);

            var constructor = GetFunction(type.GetMethod("constructor"));
            constructor.Set("prototype", prototype);
            prototype.Set("constructor", constructor);

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                if (method.Name != "constructor") {
                    try {
                        prototype.Set(method.Name.Replace("@", ""), GetFunction(method));
                    } catch { }
                }
            }

            Scope.DeclareVariable(type.Name, Javascript.DeclarationScope.Global, true, constructor);
            _prototypes[type.Name] = constructor;
        }

        public void RegisterFunctions(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                try {
                    Scope.DeclareVariable(method.Name.Replace("@", ""), Javascript.DeclarationScope.Global, true, GetFunction(method));
                } catch { }
            }
        }
    }
}
