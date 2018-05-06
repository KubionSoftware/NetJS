using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ExternalFunction : Function {
        public string Name;
        public Func<Constant, Constant[], Scope, Constant> Function;

        public ExternalFunction(string name, Func<Constant, Constant[], Scope, Constant> function, Scope scope) : base(scope) {
            Name = name;
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

                var result = Function(_this, arguments, functionScope);
                return result;
            }

            return base.Call(other, _this, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append($"[[{Name}]](){{}}");
        }

        public override string ToDebugString() {
            return $"[[{Name}]](){{}}";
        }
    }
}
