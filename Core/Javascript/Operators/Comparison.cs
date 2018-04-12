using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class LessThan : BinaryOperator {
        public LessThan() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LessThan(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LessThan + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "less than";
        }
    }

    public class LessThanEquals : BinaryOperator {
        public LessThanEquals() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LessThanEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LessThanEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "less than equals";
        }
    }

    public class GreaterThan : BinaryOperator {
        public GreaterThan() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.GreaterThan(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.GreaterThan + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "greater than";
        }
    }

    public class GreaterThanEquals : BinaryOperator {
        public GreaterThanEquals() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.GreaterThanEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.GreaterThanEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "greater than equals";
        }
    }

    public class Equals : BinaryOperator {
        public Equals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Equals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Equals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "equals";
        }
    }

    public class NotEquals : BinaryOperator {
        public NotEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.NotEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.NotEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "not equals";
        }
    }

    public class StrictEquals : BinaryOperator {
        public StrictEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.StrictEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.StrictEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "strict equals";
        }
    }

    public class StrictNotEquals : BinaryOperator {
        public StrictNotEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.StrictNotEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.StrictNotEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "strict not equals";
        }
    }
}
