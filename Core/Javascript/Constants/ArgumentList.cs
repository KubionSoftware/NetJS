using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ArgumentList : Constant {
        public IList<Expression> Arguments;

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.GroupOpen);

            for (var i = 0; i < Arguments.Count; i++) {
                if (i > 0) builder.Append(", ");
                Arguments[i].Uneval(builder, depth);
            }

            builder.Append(Tokens.GroupClose);
        }

        public override string ToDebugString() {
            var builder = new StringBuilder();
            Uneval(builder, 0);
            return builder.ToString();
        }
    }
}
