using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetJS.Core {
    public class Realm {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-code-realms

        public Object GlobalObject;
        public LexicalEnvironment GlobalEnv;
        
        private Dictionary<string, Object> _prototypes = new Dictionary<string, Object>();

        private Agent _agent;
        public Agent GetAgent() => _agent;

        public Object GetPrototype(string name) {
            if (!_prototypes.ContainsKey(name)) {
                throw new InternalError($"Could not find prototype '{name}'");
            }
            return _prototypes[name];
        }

        public Realm() {
            _agent = new Agent(this);
            CreateIntrinsics();
        }

        public void DeclareVariable(string keyString, Constant value) {
            var key = new String(keyString);

            if (!GlobalEnv.Record.HasBinding(key)) {
                GlobalEnv.Record.CreateMutableBinding(key, true);
                GlobalEnv.Record.InitializeBinding(key, value);
            } else {
                GlobalEnv.Record.SetMutableBinding(key, value, false);
            }
        }

        MethodInfo GetMethodInfo(Func<Constant, Constant[], Agent, Constant> f) {
            return f.Method;
        }

        public void CreateIntrinsics() {
            // TODO: ECMAScript

            // Object and Function need to be bootstrapped because they are dependent on each other
            var objectPrototype = new Object(null);
            var functionPrototype = new Object(objectPrototype);

            var objectConstructor = GetFunction("Object", GetMethodInfo(API.ObjectAPI.constructor), functionPrototype);
            objectConstructor.Set(new String("prototype"), objectPrototype);
            objectPrototype.Set(new String("constructor"), objectConstructor);

            var functionConstructor = GetFunction("Function", GetMethodInfo(API.FunctionAPI.constructor), functionPrototype);
            functionConstructor.Set(new String("prototype"), functionPrototype);
            functionPrototype.Set(new String("constructor"), functionConstructor);

            GlobalObject = new Object(objectPrototype);
            GlobalEnv = LexicalEnvironment.NewGlobalEnvironment(GlobalObject, GlobalObject);
            _agent.Running.Lex = GlobalEnv;

            DeclareVariable("Object", objectConstructor);
            _prototypes["Object"] = objectConstructor;

            DeclareVariable("Function", functionConstructor);
            _prototypes["Function"] = functionConstructor;

            RegisterType(typeof(API.ObjectAPI), "Object");
            RegisterType(typeof(API.FunctionAPI), "Function");
            RegisterType(typeof(API.ArrayAPI), "Array");
            RegisterType(typeof(API.Uint8ArrayAPI), "Uint8Array");
            RegisterType(typeof(API.DateAPI), "Date");
            RegisterType(typeof(API.StringAPI), "String");
            RegisterType(typeof(API.NumberAPI), "Number");
            RegisterType(typeof(API.BooleanAPI), "Boolean");
            RegisterType(typeof(API.RegExpAPI), "RegExp");

            RegisterClass(typeof(API.JSONAPI), "JSON");
            RegisterClass(typeof(API.MathAPI), "Math");

            RegisterFunctions(typeof(API.FunctionsAPI));

            GlobalObject = Tool.Construct("Object", _agent);
        }

        public void RegisterDLL(string file) {
            var dll = Assembly.LoadFile(file);

            foreach (System.Type type in dll.GetExportedTypes()) {
                RegisterClass(type, type.Name);
            }
        }

        private ExternalFunction GetFunction(string className, MethodInfo info, Object prototype = null) {
            return new ExternalFunction(className + "." + info.Name,
                (Func<
                    Constant,
                    Constant[],
                    Agent,
                    Constant
                >)info.CreateDelegate(typeof(Func<
                    Constant,
                    Constant[],
                    Agent,
                    Constant
                >)), _agent, prototype);
        }

        public void RegisterClass(System.Type type, string name) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var obj = Tool.Construct("Object", _agent);

            foreach (var method in methods) {
                try {
                    obj.Set(new String(method.Name.Replace("@", "")), GetFunction(name, method));
                } catch { }
            }

            DeclareVariable(name, obj);
        }

        public void RegisterType(System.Type type, string name) {
            var prototype = Tool.Construct("Object", _agent);

            var constructor = GetFunction(name, type.GetMethod("constructor"));
            constructor.Set(new String("prototype"), prototype);
            prototype.Set(new String("constructor"), constructor);

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                if (method.Name != "constructor") {
                    try {
                        if (method.GetCustomAttributes(typeof(API.StaticFunction), false).Any()) {
                            constructor.Set(new String(method.Name.Replace("@", "")), GetFunction(name, method));
                        } else {
                            prototype.Set(new String(method.Name.Replace("@", "")), GetFunction(name, method));
                        }
                    } catch { }
                }
            }

            DeclareVariable(name, constructor);
            _prototypes[name] = constructor;
        }

        public void RegisterFunctions(System.Type type) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                try {
                    DeclareVariable(method.Name.Replace("@", ""), GetFunction(type.Name, method));
                } catch { }
            }
        }
    }
}
