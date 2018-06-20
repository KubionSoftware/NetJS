using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public enum ThisMode {
        Lexical,
        Strict,
        Global
    }

    public enum FunctionKind {
        Normal,
        Method,
        Arrow
    }

    public abstract class Function : Object {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#table-27

        public LexicalEnvironment Environment;
        public Realm Realm;
        public ParameterList FormalParameters;
        public string FunctionKind; // Either 'normal', 'classConstructor', 'generator' or 'async'
        public Statement ECMAScriptCode;
        public string ConstructorKind; // Either 'base' or 'derived'
        public ScriptRecord ScriptOrModule;
        public ThisMode ThisMode;
        public bool Strict;
        public Object HomeObject;

        public Function(Object proto) : base(proto) { }

        public static Function FunctionInitialize(Function f, FunctionKind kind, ParameterList parameterList, Statement body, LexicalEnvironment scope, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-functioninitialize

            var len = parameterList.ExpectedArgumentCount();
            f.DefinePropertyOrThrow(new String("length"), new DataProperty() {
                Value = new Number(len),
                Writable = false,
                Enumerable = false,
                Configurable = true
            });

            var strict = f.Strict;
            f.Environment = scope;
            f.FormalParameters = parameterList;
            f.ECMAScriptCode = body;
            f.ScriptOrModule = agent.Running.ScriptOrModule;

            if (kind == Core.FunctionKind.Arrow) {
                f.ThisMode = ThisMode.Lexical;
            } else if (strict) {
                f.ThisMode = ThisMode.Strict;
            } else {
                f.ThisMode = ThisMode.Global;
            }

            return f;
        }

        public static Function FunctionAllocate(Object functionPrototype, bool strict, string functionKind, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-functionallocate

            var needsConstruct = functionKind == "normal";
            if (functionKind == "non-constructor") functionKind = "normal";

            var f = new InternalFunction(functionPrototype);
            f.Strict = strict;
            f.FunctionKind = functionKind;
            f.Extensible = true;
            f.Realm = agent.Running.Realm;

            return f;
        }

        public static Function FunctionCreate(FunctionKind kind, ParameterList parameterList, Statement body, LexicalEnvironment scope, bool strict, Agent agent, Object prototype = null) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-functioncreate

            if (prototype == null) {
                prototype = Tool.Prototype("Function", agent);
            }

            var allocKind = kind != Core.FunctionKind.Normal ? "non-constructor" : "normal";
            var f = FunctionAllocate(prototype, strict, allocKind, agent);
            return FunctionInitialize(f, kind, parameterList, body, scope, agent);
        }

        public Context PrepareForOrdinaryCall(Object newTarget, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-prepareforordinarycall

            var callerContext = agent.Running;

            var calleeContext = new Context(Realm, callerContext.Buffer);
            calleeContext.Function = this;
            calleeContext.ScriptOrModule = ScriptOrModule;

            var localEnv = LexicalEnvironment.NewFunctionEnvironment(this, newTarget);
            calleeContext.Lex = localEnv;
            calleeContext.Var = localEnv;

            // TODO: suspend callerContext

            agent.Push(calleeContext);
            return calleeContext;
        }

        public Completion OrdinaryCallBindThis(Context calleeContext, Constant thisArgument, Agent agent) {
            var thisMode = ThisMode;
            if (thisMode == ThisMode.Lexical) return Static.NormalCompletion;

            var calleeRealm = Realm;
            var localEnv = calleeContext.Lex;
            Constant thisValue;

            if (thisMode == ThisMode.Strict) {
                thisValue = thisArgument;
            } else {
                if (thisArgument is Undefined || thisArgument is Null) {
                    var globalEnv = calleeRealm.GlobalEnv;
                    var globalEnvRec = (GlobalEnvironmentRecord)globalEnv.Record;
                    thisValue = globalEnvRec.GlobalThisValue;
                } else {
                    thisValue = Convert.ToObject(thisArgument, agent);
                }
            }

            var envRec = localEnv.Record;
            return ((FunctionEnvironmentRecord)envRec).BindThisValue(thisValue);
        }

        public static bool HasDuplicates<T>(T[] values) {
            var uniqueValues = new HashSet<T>();
            foreach (T d in values) {
                if (uniqueValues.Contains(d)) {
                    return true;
                }
                uniqueValues.Add(d);
            }
            return false;
        }

        public Completion FunctionDeclarationInstantiation(Constant[] arguments, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-functiondeclarationinstantiation

            var calleeContext = agent.Running;
            var env = calleeContext.Lex;
            var envRec = env.Record;

            var parameterNames = FormalParameters.BoundNames();
            var hasDuplicates = HasDuplicates(parameterNames);
            
            // TODO: a lot

            for(var i = 0; i < parameterNames.Length; i++) {
                envRec.CreateMutableBinding(parameterNames[i], false, agent);
                envRec.InitializeBinding(parameterNames[i], i < arguments.Length ? arguments[i] : Static.Undefined, agent);
            }

            var argumentsArray = new Array(arguments.Length, agent);
            argumentsArray.AddRange(arguments);

            envRec.CreateImmutableBinding(new String("arguments"), true, agent);
            envRec.InitializeBinding(new String("arguments"), argumentsArray, agent);

            DeclarationFinder.FindVarDeclarations(ECMAScriptCode, (dn, isConstant) => {
                if (isConstant) {
                    envRec.CreateImmutableBinding(dn, true, agent);
                } else {
                    envRec.CreateMutableBinding(dn, false, agent);
                }
                envRec.InitializeBinding(dn, Static.Undefined, agent);
            });

            return Static.NormalCompletion;
        }

        public Completion EvaluateBody(Constant[] arguments, Agent agent) {
            FunctionDeclarationInstantiation(arguments, agent);
            return ECMAScriptCode.Evaluate(agent);
        }

        public abstract Constant Call(Constant thisValue, Agent agent, Constant[] arguments = null);
    }
}
