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

            if (!GlobalEnv.Record.HasBinding(key, _agent)) {
                GlobalEnv.Record.CreateMutableBinding(key, true, _agent);
                GlobalEnv.Record.InitializeBinding(key, value, _agent);
            } else {
                GlobalEnv.Record.SetMutableBinding(key, value, false, _agent);
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

            RegisterForeignNamespace("System");

            GlobalObject = Tool.Construct("Object", _agent);
        }

        public void RegisterDLL(string file) {
            var dll = Assembly.LoadFile(file);

            foreach (System.Type type in dll.GetExportedTypes()) {
                RegisterClass(type, type.Name);
            }
        }

        private Dictionary<System.Type, Object> _foreigns = new Dictionary<System.Type, Object>();
        private Dictionary<string, Object> _namespaces = new Dictionary<string, Object>();

        private Object CreateForeignObject(object foreign) {
            var obj = Tool.Construct("Object", _agent);
            obj.Prototype = _foreigns[foreign.GetType()];
            obj.Set("[[Foreign]]", new Foreign(foreign));
            return obj;
        }

        public void RegisterForeignNamespace(string ns) {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes()).Where(t => t.Namespace == ns);
            foreach (var type in types) {
                RegisterForeignType(type);
            }
        }

        public Object GetNamespace(System.Type type) {
            var ns = type.Namespace;
            var parts = ns.Split('.');

            if (parts.Length == 0) throw new InternalError("No namespace for type " + type.ToString());

            Object b;
            if (_namespaces.ContainsKey(parts[0])) {
                b = _namespaces[parts[0]];
            } else {
                b = Tool.Construct("Object", _agent);
                _namespaces[parts[0]] = b;
                DeclareVariable(parts[0], b);
            }

            for (var i = 1; i < parts.Length; i++) {
                if (b.HasProperty(new String(parts[i]))) {
                    b = b.Get(parts[i], _agent) as Object;
                } else {
                    var obj = Tool.Construct("Object", _agent); ;
                    b.Set(parts[i], obj);
                    b = obj;
                }
            }

            return b;
        }

        public void RegisterForeignType(System.Type type) {
            var prototype = Tool.Construct("Object", _agent);
            
            var constructor = new ExternalFunction(type.FullName, (_this, arguments, agent) => {
                var args = ToForeignTypes(arguments);
                var con = type.GetConstructor(args.Select(a => a.GetType()).ToArray());
                if (con == null) throw new TypeError("There is no foreign constructor for the given arguments");
                return CreateForeignObject(con.Invoke(args));
            }, _agent);
            constructor.Set(new String("prototype"), prototype);
            prototype.Set(new String("constructor"), constructor);

            foreach (var method in type.GetMethods()) {
                if (method.IsStatic) {
                    constructor.Set(method.Name, new ExternalFunction(type.FullName + "." + method.Name, (_this, arguments, agent) => {
                        var args = ToForeignTypes(arguments);
                        var m = type.GetMethod(method.Name, BindingFlags.Static | BindingFlags.Public, null, args.Select(a => a.GetType()).ToArray(), null);
                        if (m == null) {
                            throw new InternalError($"Can't find method with name {method.Name} for type {type.FullName}");
                        }
                        return FromForeignType(m.Invoke(null, args));
                    }, _agent));
                } else {
                    prototype.Set(method.Name, new ExternalFunction(type.FullName + "." + method.Name, (_this, arguments, agent) => {
                        var foreign = ((_this as Object).Get("[[Foreign]]", agent) as Foreign).Value;
                        var args = ToForeignTypes(arguments);
                        var m = type.GetMethod(method.Name, args.Select(a => a.GetType()).ToArray());
                        if (m == null) {
                            throw new InternalError($"Can't find method with name {method.Name} for type {type.FullName}");
                        }
                        return FromForeignType(m.Invoke(foreign, args));
                    }, _agent));
                }
            }

            foreach (var property in type.GetProperties()) {
                var method = property.GetMethod;
                if (method.IsStatic) {
                    constructor.DefineOwnProperty(new String(property.Name), new AccessorProperty() {
                        Get = new ExternalFunction(type.FullName + "." + property.Name, (_this, arguments, agent) => {
                            var args = ToForeignTypes(arguments);
                            return FromForeignType(property.GetGetMethod(false).Invoke(null, args));
                        }, _agent),
                        Configurable = true,
                        Enumerable = true
                    });
                } else {
                    prototype.DefineOwnProperty(new String(property.Name), new AccessorProperty() {
                        Get = new ExternalFunction(type.FullName + "." + property.Name, (_this, arguments, agent) => {
                            var foreign = ((_this as Object).Get("[[Foreign]]", agent) as Foreign).Value;
                            var args = ToForeignTypes(arguments);
                            return FromForeignType(property.GetGetMethod(false).Invoke(foreign, args));
                        }, _agent),
                        Configurable = true,
                        Enumerable = true
                    });
                }
            }

            var ns = GetNamespace(type);
            ns.Set(type.Name, constructor);

            _foreigns[type] = prototype;
        }

        private object[] ToForeignTypes(Constant[] arguments) {
            var args = new object[arguments.Length];
            for (var i = 0; i < arguments.Length; i++) args[i] = ToForeignType(arguments[i]);
            return args;
        }

        private object ToForeignType(Constant v) {
            switch (v) {
                case Undefined u: return null;
                case Null nu: return null;
                case Number n: {
                    if(n.Value % 1 == 0) {
                        return (int)n.Value;
                    } else {
                        return n.Value;
                    }
                }
                case Boolean b: return b.Value;
                case String s: return s.Value;
            }

            throw new TypeError($"Can't convert {v.ToDebugString()} to foreign type");
        }

        private Constant FromForeignType(object v) {
            switch (v) {
                case null: return Static.Undefined;
                case byte bt: return new Number(bt);
                case short sh: return new Number(sh);
                case int i: return new Number(i);
                case long l: return new Number(l);
                case float f: return new Number(f);
                case double d: return new Number(d);
                case bool b: return Boolean.Create(b);
                case string s: return new String(s);
            }

            if (_foreigns.ContainsKey(v.GetType())) {
                return CreateForeignObject(v);
            }

            throw new TypeError($"Can't convert {v.ToString()} to internal type");
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
                var parameters = method.GetParameters();
                if (
                    parameters.Length == 3 && 
                    parameters[0].GetType() == typeof(Constant) && 
                    parameters[1].GetType() == typeof(Constant[]) && 
                    parameters[2].GetType() == typeof(Agent)
                ) {
                    try {
                        obj.Set(new String(method.Name.Replace("@", "")), GetFunction(name, method));
                    } catch { }
                }
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
