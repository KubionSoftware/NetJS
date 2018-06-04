using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ArgumentList : Expression {

        public Expression[] Arguments { get; private set; }

        public ArgumentList(params Expression[] expressions) {
            Arguments = expressions;
        }

        public new Constant[] Evaluate(Agent agent) {
            if (Arguments == null) return new Constant[] { };

            var constants = new Constant[Arguments.Length];
            for (var i = 0; i < Arguments.Length; i++) constants[i] = Arguments[i].Evaluate(agent);
            return constants;
        }

        public override string ToDebugString() {
            return string.Join(", ", Arguments.Select(a => a.ToDebugString()));
        }
    }
}
