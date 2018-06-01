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

        public override Constant Call(Constant[] arguments, Constant thisValue, Scope scope) {
            // TODO: store function node
            var functionScope = new Scope(Scope, scope, Body, ScopeType.Function, scope.Buffer);

            functionScope.DeclareVariable("this", DeclarationScope.Function, true, thisValue);

            for (var i = 0; i < arguments.Length && i < Parameters.Parameters.Count; i++) {
                var value = arguments[i];
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
    }
}
