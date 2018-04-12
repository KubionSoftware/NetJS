using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ParameterList : Node {
        public IList<Variable> Parameters = new List<Variable>();

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("(");

            for (var i = 0; i < Parameters.Count; i++) {
                if (i > 0) builder.Append(", ");
                Parameters[i].Uneval(builder, depth);
            }

            builder.Append(")");
        }
    }
}
