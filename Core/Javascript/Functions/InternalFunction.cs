using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class InternalFunction : Function {
        public string Name;
        public Type Type;
        public ParameterList Parameters;
        public Block Body;

        public InternalFunction(Scope scope) : base(scope) { }

        public override Constant Call(Constant other, Constant _this, Scope scope) {
            if (other is ArgumentList) {
                var argumentList = (ArgumentList)other;
                var functionScope = new Scope(Scope, scope, this, ScopeType.Function, scope.Buffer);

                functionScope.DeclareVariable("this", DeclarationScope.Function, true, _this);

                for (var i = 0; i < argumentList.Arguments.Length && i < Parameters.Parameters.Count; i++) {
                    var value = argumentList.Arguments[i].Execute(scope);
                    functionScope.DeclareVariable(Parameters.Parameters[i].Name, DeclarationScope.Function, false, value, Parameters.Parameters[i].Type);
                }

                var result = Body.Execute(functionScope).Constant;
                if (Type != null) {
                    if (!Type.Check(result, scope)) {
                        throw new TypeError($"Function cannot return value of type '{result.GetType()}', must return '{Type}'");
                    }
                }

                return result;
            }

            return base.Call(other, _this, scope);
        }
    }
}
