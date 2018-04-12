using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Assignment : BinaryOperator {
        public Assignment() : base(2) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Assignment(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            // TODO: error if left not variable or access
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope);
            return Execute(left, right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Assign + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }
}
