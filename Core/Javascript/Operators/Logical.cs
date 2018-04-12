using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class LogicalNot : UnaryRightOperator {
        public LogicalNot() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.LogicalNot(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.LogicalNot);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical not";
        }
    }

    public class LogicalAnd : BinaryOperator {
        public LogicalAnd() : base(5) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LogicalAnd(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LogicalAnd + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical and";
        }
    }

    public class LogicalOr : BinaryOperator {
        public LogicalOr() : base(4) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LogicalOr(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LogicalOr + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical or";
        }
    }
}
