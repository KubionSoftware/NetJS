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

            Scope.SetVariable("Object", objectConstructor);
            _prototypes["Object"] = objectConstructor;

            Scope.SetVariable("Function", functionConstructor);
            _prototypes["Function"] = functionConstructor;

            RegisterType(typeof(Internal.Object));
            RegisterType(typeof(Internal.Function));
            RegisterType(typeof(Internal.Array));
            RegisterType(typeof(Internal.Date));
            RegisterType(typeof(Internal.String));
            RegisterType(typeof(Internal.Number));
            RegisterType(typeof(Internal.Boolean));
            RegisterType(typeof(Internal.RegExp));

            RegisterClass(typeof(Internal.JSON));
            RegisterClass(typeof(Internal.Math));

            RegisterFunctions(typeof(Internal.Functions));
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
                obj.Set(method.Name.Replace("@", ""), GetFunction(method));
            }

            new Javascript.Variable(type.Name).Assignment(obj, Scope);
        }

        public void RegisterType(Type type) {
            var prototype = Tool.Construct("Object", Scope);

            var constructor = GetFunction(type.GetMethod("constructor"));
            constructor.Set("prototype", prototype);
            prototype.Set("constructor", constructor);

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                if (method.Name != "constructor") {
                    prototype.Set(method.Name.Replace("@", ""), GetFunction(method));
                }
            }

            Scope.SetVariable(type.Name, constructor);
            _prototypes[type.Name] = constructor;
        }

        public void RegisterFunctions(Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                Scope.SetVariable(method.Name.Replace("@", ""), GetFunction(method));
            }
        }
    }
}
