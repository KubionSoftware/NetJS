using System;
using System.Text;

namespace NetJS.Core.Javascript {
    public class ExternalFunction : Function {
        public Func<Constant, Constant[], Scope, Constant> Function;

        public ExternalFunction(Func<Constant, Constant[], Scope, Constant> function, Scope scope) : base(scope) {
            Function = function;
        }

        public override Constant Call(Constant other, Constant _this, Scope scope) {
            if (other is ArgumentList) {
                var argumentList = (ArgumentList)other;
                var arguments = new Constant[argumentList.Arguments.Count];

                for (var i = 0; i < argumentList.Arguments.Count; i++) {
                    var value = argumentList.Arguments[i].Execute(scope);
                    arguments[i] = value;
                }

                var functionScope = new Scope(Scope, scope, this, ScopeType.Function, scope.Buffer);

                // TODO: THIS IS A MEGA-HACK, REMOVE AS SOON AS POSSIBLE!!!
                foreach (var key in scope.Variables) {
                    if (key.StartsWith("__") && key.EndsWith("__")) {
                        functionScope.SetVariable(key, scope.GetVariable(key));
                    }
                }

                var result = Function(_this, arguments, functionScope);
                return result;
            }

            return base.Call(other, _this, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("[[externalFunction]](){}");
        }
    }

    public class Constructor : Constant {
        public Function Function;

        public Constructor(Function function) {
            Function = function;
        }

        public override Constant Call(Constant other, Constant _this, Scope scope) {
            var result = Function.Call(other, _this, scope);
            if (!(result is Undefined)) {
                return result;
            } else {
                return _this;
            }
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("[[constructor]](){}");
        }

        public override string ToDebugString() {
            return "[[constructor]]";
        }
    }

    public class InternalFunction : Function {
        public string Name;
        public string Type;
        public ParameterList Parameters;
        public Block Body;

        public InternalFunction(Scope scope) : base(scope) { }

        public override Constant Call(Constant other, Constant _this, Scope scope) {
            if (other is ArgumentList) {
                var argumentList = (ArgumentList)other;
                var functionScope = new Scope(Scope, scope, this, ScopeType.Function, scope.Buffer);

                functionScope.SetVariable("this", _this);

                for (var i = 0; i < argumentList.Arguments.Count && i < Parameters.Parameters.Count; i++) {
                    var value = argumentList.Arguments[i].Execute(scope);
                    Parameters.Parameters[i].Assignment(value, functionScope);
                }

                var result = Body.Execute(functionScope).Constant;
                if(Type != null && Type.Length > 0) {
                    if(!Tool.CheckType(result, Type)) {
                        throw new TypeError($"Function cannot return value of type '{result.GetType()}', must return '{Type}'");
                    }
                }

                return result;
            }

            return base.Call(other, _this, scope);
        }

        public static void UnevalFunction(StringBuilder builder, int depth, string name, ParameterList parameters, Block body) {
            builder.Append(Tokens.Variable + " " + name + " " + Tokens.Assign + " ");
            builder.Append(Tokens.Function);

            parameters.Uneval(builder, depth);
            builder.Append(Tokens.BlockOpen);

            NewLine(builder, depth + 1);
            body.Uneval(builder, depth + 1);

            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            UnevalFunction(builder, depth, Name, Parameters, Body);
        }
    }

    public abstract class Function : Object {
        public Scope Scope { get; }

        public Function(Scope scope) : base(Tool.Prototype("Function", scope)) {
            Scope = scope;
            Set("prototype", Tool.Construct("Object", scope));
        }

        public override Constant New(Scope scope) {
            return new Constructor(this);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("function");
        }
    }
}