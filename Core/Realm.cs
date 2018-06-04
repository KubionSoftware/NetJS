using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetJS.Core.Javascript {
    public class Realm {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-code-realms

        public Object GlobalObject;
        public LexicalEnvironment GlobalEnv;
        
        private Dictionary<string, Javascript.Object> _prototypes = new Dictionary<string, Javascript.Object>();

        public Javascript.Object GetPrototype(string name) {
            if (!_prototypes.ContainsKey(name)) {
                throw new Javascript.InternalError($"Could not find prototype '{name}'");
            }
            return _prototypes[name];
        }

        public Realm() {
            CreateIntrinsics();

        }

        public void CreateIntrinsics() {
            // Object and Function need to be bootstrapped because they are dependent on each other
            var objectPrototype = new Javascript.Object(null);
            var functionPrototype = new Javascript.Object(objectPrototype);

            var objectConstructor = new Javascript.Object(functionPrototype);
            objectConstructor.Set("prototype", objectPrototype);
            objectPrototype.Set("constructor", objectConstructor);

            var functionConstructor = new Javascript.Object(functionPrototype);
            functionConstructor.Set("prototype", functionPrototype);
            functionPrototype.Set("constructor", functionConstructor);

            EngineScope.DeclareVariable("Object", Javascript.DeclarationScope.Engine, true, objectConstructor);
            _prototypes["Object"] = objectConstructor;

            EngineScope.DeclareVariable("Function", Javascript.DeclarationScope.Engine, true, functionConstructor);
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

            GlobalObject = Tool.Construct("Object", EngineScope);
        }

        public void RegisterDLL(string file) {
            var dll = Assembly.LoadFile(file);

            foreach (Type type in dll.GetExportedTypes()) {
                RegisterClass(type);
            }
        }

        private Javascript.ExternalFunction GetFunction(string className, MethodInfo info) {
            return new Javascript.ExternalFunction(className + "." + info.Name,
                (Func<
                    Javascript.Constant,
                    Javascript.Constant[],
                    Javascript.LexicalEnvironment,
                    Javascript.Constant
                >)info.CreateDelegate(typeof(Func<
                    Javascript.Constant,
                    Javascript.Constant[],
                    Javascript.LexicalEnvironment,
                    Javascript.Constant
                >)), EngineScope);
        }

        public void RegisterClass(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var obj = Tool.Construct("Object", EngineScope);

            foreach (var method in methods) {
                try {
                    obj.Set(method.Name.Replace("@", ""), GetFunction(type.Name, method));
                } catch { }
            }

            EngineScope.DeclareVariable(type.Name, Javascript.DeclarationScope.Engine, true, obj);
        }

        public void RegisterType(Type type) {
            var prototype = Tool.Construct("Object", EngineScope);

            var constructor = GetFunction(type.Name, type.GetMethod("constructor"));
            constructor.Set("prototype", prototype);
            prototype.Set("constructor", constructor);

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                if (method.Name != "constructor") {
                    try {
                        if (method.GetCustomAttributes(typeof(API.StaticFunction), false).Any()) {
                            constructor.Set(method.Name.Replace("@", ""), GetFunction(type.Name, method));
                        } else {
                            prototype.Set(method.Name.Replace("@", ""), GetFunction(type.Name, method));
                        }
                    } catch { }
                }
            }

            EngineScope.DeclareVariable(type.Name, Javascript.DeclarationScope.Engine, true, constructor);
            _prototypes[type.Name] = constructor;
        }

        public void RegisterFunctions(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                try {
                    EngineScope.DeclareVariable(method.Name.Replace("@", ""), Javascript.DeclarationScope.Engine, true, GetFunction(type.Name, method));
                } catch { }
            }
        }
    }
}
