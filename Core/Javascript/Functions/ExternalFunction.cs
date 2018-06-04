using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ExternalFunction : Function {
        public string Name;
        public Func<Constant, Constant[], LexicalEnvironment, Constant> Function;

        public ExternalFunction(string name, Func<Constant, Constant[], LexicalEnvironment, Constant> function, LexicalEnvironment lex) : base(lex) {
            Name = name;
            Function = function;
        }

        public override Constant Call(Constant[] arguments, Constant thisValue, LexicalEnvironment lex) {
            var functionScope = new LexicalEnvironment(Context, lex, null, EnvironmentType.Function, lex.Buffer);

            var result = Function(thisValue, arguments, functionScope);
            return result;
        }

        public override string ToDebugString() {
            return $"[[{Name}]](){{}}";
        }
    }
}
