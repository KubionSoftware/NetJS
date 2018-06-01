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

        public override Constant Call(Constant[] arguments, Constant thisValue, Scope scope) {
            var functionScope = new Scope(Scope, scope, null, ScopeType.Function, scope.Buffer);

            var result = Function(thisValue, arguments, functionScope);
            return result;
        }

        public override string ToDebugString() {
            return $"[[{Name}]](){{}}";
        }
    }
}
