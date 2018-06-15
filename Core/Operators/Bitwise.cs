using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class BitwiseNot : UnaryRightOperator {
        public BitwiseNot() : base(14) { }

        public override Constant Evaluate(Constant expr, Agent agent) {
            var oldValue = Convert.ToInt32(References.GetValue(expr, agent), agent);
            return new Number(~oldValue);
        }

        public override string ToDebugString() {
            return "bitwise not";
        }
    }

    public class LeftShift : BinaryOperator {
        public LeftShift() : base(12) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-left-shift-operator

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lnum = Convert.ToInt32(lval, agent);
            var rnum = Convert.ToUint32(rval, agent);
            var shiftCount = rnum & 0x1F;

            return new Number(lnum << shiftCount);
        }

        public override string ToDebugString() {
            return "left shift";
        }
    }

    public class RightShift : BinaryOperator {
        public RightShift() : base(12) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-signed-right-shift-operator

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lnum = Convert.ToInt32(lval, agent);
            var rnum = Convert.ToUint32(rval, agent);
            var shiftCount = rnum & 0x1F;

            return new Number(lnum >> shiftCount);
        }

        public override string ToDebugString() {
            return "right shift";
        }
    }

    public abstract class BitwiseOperator : BinaryOperator {
        public BitwiseOperator(int precedence) : base(precedence) { }

        public abstract int Operation(int left, int right);

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-binary-bitwise-operators

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lnum = Convert.ToInt32(lval, agent);
            var rnum = Convert.ToInt32(rval, agent);

            return new Number(Operation(lnum, rnum));
        }

        public override string ToDebugString() {
            return "multiplicative";
        }
    }

    public class BitwiseAnd : BitwiseOperator {
        public BitwiseAnd() : base(8) { }

        public override int Operation(int left, int right) {
            return left & right;
        }

        public override string ToDebugString() {
            return "bitwise and";
        }
    }

    public class BitwiseXor : BitwiseOperator {
        public BitwiseXor() : base(7) { }

        public override int Operation(int left, int right) {
            return left ^ right;
        }

        public override string ToDebugString() {
            return "bitwise xor";
        }
    }

    public class BitwiseOr : BitwiseOperator {
        public BitwiseOr() : base(6) { }

        public override int Operation(int left, int right) {
            return left | right;
        }

        public override string ToDebugString() {
            return "bitwise or";
        }
    }
}
