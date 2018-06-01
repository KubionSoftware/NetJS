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

        public override string ToDebugString() {
            return string.Join(", ", Arguments.Select(a => a.ToDebugString()));
        }
    }
}
