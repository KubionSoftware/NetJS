using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
