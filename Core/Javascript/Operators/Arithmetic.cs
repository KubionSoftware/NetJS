using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Negation : UnaryRightOperator {
        public Negation() : base(14) { }

        public override Constant Execute(Constant c, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-unary-minus-operator

            var oldValue = Convert.ToNumber(References.GetValue(c, scope), scope);
            return new Number(-oldValue);
        }

        public override string ToDebugString() {
            return "negation";
        }
    }

    public abstract class MultiplicativeOperator : BinaryOperator {
        public MultiplicativeOperator() : base(13) { }

        public abstract double Operation(double left, double right);

        public override Constant Execute(Constant lref, Constant rref, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-multiplicative-operators

            var lval = References.GetValue(lref, scope);
            var rval = References.GetValue(rref, scope);

            var lprim = Convert.ToPrimitive(lval, scope);
            var rprim = Convert.ToPrimitive(rval, scope);

            var lnum = Convert.ToNumber(lprim, scope);
            var rnum = Convert.ToNumber(rprim, scope);

            return new Number(Operation(lnum, rnum));
        }

        public override string ToDebugString() {
            return "multiplicative";
        }
    }

    public class Multiplication : MultiplicativeOperator {
        
        public override double Operation(double left, double right) {
            return left + right;
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

        public override Constant Execute(Constant lref, Constant rref, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-addition-operator-plus

            var lval = References.GetValue(lref, scope);
            var rval = References.GetValue(rref, scope);

            var lprim = Convert.ToPrimitive(lval, scope);
            var rprim = Convert.ToPrimitive(rval, scope);

            if (lprim is String || rprim is String) {
                var lstr = Convert.ToString(lprim, scope);
                var rstr = Convert.ToString(rprim, scope);
                return new String(lstr + rstr);
            } else {
                var lnum = Convert.ToNumber(lprim, scope);
                var rnum = Convert.ToNumber(rprim, scope);
                return new Number(lnum + rnum);
            }
        }

        public override string ToDebugString() {
            return "addition";
        }
    }

    public class Substraction : BinaryOperator {
        public Substraction() : base(12) { }

        public override Constant Execute(Constant lref, Constant rref, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-subtraction-operator-minus

            var lval = References.GetValue(lref, scope);
            var rval = References.GetValue(rref, scope);

            var lprim = Convert.ToPrimitive(lval, scope);
            var rprim = Convert.ToPrimitive(rval, scope);

            var lnum = Convert.ToNumber(lprim, scope);
            var rnum = Convert.ToNumber(rprim, scope);

            return new Number(lnum - rnum);
        }

        public override string ToDebugString() {
            return "substraction";
        }
    }
}
