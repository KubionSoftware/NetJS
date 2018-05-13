using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class BitwiseNot : UnaryRightOperator {
        public BitwiseNot() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.BitwiseNot(scope);
        }

        public override string ToDebugString() {
            return "bitwise not";
        }
    }

    public class LeftShift : BinaryOperator {
        public LeftShift() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LeftShift(right, scope);
        }

        public override string ToDebugString() {
            return "left shift";
        }
    }

    public class RightShift : BinaryOperator {
        public RightShift() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.RightShift(right, scope);
        }

        public override string ToDebugString() {
            return "right shift";
        }
    }

    public class BitwiseAnd : BinaryOperator {
        public BitwiseAnd() : base(8) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseAnd(right, scope);
        }

        public override string ToDebugString() {
            return "bitwise and";
        }
    }

    public class BitwiseXor : BinaryOperator {
        public BitwiseXor() : base(7) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseXor(right, scope);
        }

        public override string ToDebugString() {
            return "bitwise xor";
        }
    }

    public class BitwiseOr : BinaryOperator {
        public BitwiseOr() : base(6) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseOr(right, scope);
        }

        public override string ToDebugString() {
            return "bitwise or";
        }
    }
}
