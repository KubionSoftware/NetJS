using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Delete : UnaryRightOperator {
        public Delete() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.Delete(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Delete + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "delete";
        }
    }

    public class In : BinaryOperator {
        public In() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.In(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.In + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "in";
        }
    }
}
