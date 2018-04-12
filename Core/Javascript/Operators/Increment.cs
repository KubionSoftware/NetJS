using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class PostfixIncrement : UnaryLeftOperator {
        public PostfixIncrement() : base(15) { }

        public override Constant Execute(Constant left, Scope scope) {
            return left.PostfixIncrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(Tokens.Increment);
        }

        public override string ToDebugString() {
            return "postfix increment";
        }
    }

    public class PostfixDecrement : UnaryLeftOperator {
        public PostfixDecrement() : base(15) { }

        public override Constant Execute(Constant left, Scope scope) {
            return left.PostfixDecrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(Tokens.Decrement);
        }

        public override string ToDebugString() {
            return "postfix decrement";
        }
    }

    public class PrefixIncrement : UnaryRightOperator {
        public PrefixIncrement() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.PrefixIncrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Increment);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "prefix increment";
        }
    }

    public class PrefixDecrement : UnaryRightOperator {
        public PrefixDecrement() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.PrefixDecrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Decrement);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "prefix decrement";
        }
    }
}
