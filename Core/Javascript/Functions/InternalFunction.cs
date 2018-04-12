using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
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
                if (Type != null && Type.Length > 0) {
                    if (!Tool.CheckType(result, Type)) {
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
}
