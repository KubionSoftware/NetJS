using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Negation : UnaryRightOperator {
        public Negation() : base(14) { }

        public override Constant Evaluate(Constant c, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-unary-minus-operator

            var oldValue = Convert.ToNumber(References.GetValue(c, agent), agent);
            return new Number(-oldValue);
        }

        public override string ToDebugString() {
            return "negation";
        }
    }

    public abstract class MultiplicativeOperator : BinaryOperator {
        public MultiplicativeOperator() : base(13) { }

        public abstract double Operation(double left, double right);

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-multiplicative-operators

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lprim = Convert.ToPrimitive(lval, agent);
            var rprim = Convert.ToPrimitive(rval, agent);

            var lnum = Convert.ToNumber(lprim, agent);
            var rnum = Convert.ToNumber(rprim, agent);

            return new Number(Operation(lnum, rnum));
        }

        public override string ToDebugString() {
            return "multiplicative";
        }
    }

    public class Multiplication : MultiplicativeOperator {
        
        public override double Operation(double left, double right) {
            return left * right;
        }

        public override string ToDebugString() {
            return "multiplication";
        }
    }

    public class Division : MultiplicativeOperator {

        public override double Operation(double left, double right) {
            return left / right;
        }

        public override string ToDebugString() {
            return "division";
        }
    }

    public class Remainder : MultiplicativeOperator {

        public override double Operation(double left, double right) {
            return left % right;
        }

        public override string ToDebugString() {
            return "remainder";
        }
    }

    public class Addition : BinaryOperator {
        public Addition() : base(12) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-addition-operator-plus

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lprim = Convert.ToPrimitive(lval, agent);
            var rprim = Convert.ToPrimitive(rval, agent);

            if (lprim is String || rprim is String) {
                var lstr = Convert.ToString(lprim, agent);
                var rstr = Convert.ToString(rprim, agent);
                return new String(lstr + rstr);
            } else {
                var lnum = Convert.ToNumber(lprim, agent);
                var rnum = Convert.ToNumber(rprim, agent);
                return new Number(lnum + rnum);
            }
        }

        public override string ToDebugString() {
            return "addition";
        }
    }

    public class Substraction : BinaryOperator {
        public Substraction() : base(12) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-subtraction-operator-minus

            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var lprim = Convert.ToPrimitive(lval, agent);
            var rprim = Convert.ToPrimitive(rval, agent);

            var lnum = Convert.ToNumber(lprim, agent);
            var rnum = Convert.ToNumber(rprim, agent);

            return new Number(lnum - rnum);
        }

        public override string ToDebugString() {
            return "substraction";
        }
    }
}
