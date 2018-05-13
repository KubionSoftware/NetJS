using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Negation : UnaryRightOperator {
        public Negation() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.Negation(scope);
        }

        public override string ToDebugString() {
            return "negation";
        }
    }

    public class Multiplication : BinaryOperator {
        public Multiplication() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Multiply(right, scope);
        }

        public override string ToDebugString() {
            return "multiplication";
        }
    }

    public class Division : BinaryOperator {
        public Division() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Divide(right, scope);
        }

        public override string ToDebugString() {
            return "division";
        }
    }

    public class Remainder : BinaryOperator {
        public Remainder() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Remainder(right, scope);
        }

        public override string ToDebugString() {
            return "remainder";
        }
    }

    public class Addition : BinaryOperator {
        public Addition() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Add(right, scope);
        }

        public override string ToDebugString() {
            return "addition";
        }
    }

    public class Substraction : BinaryOperator {
        public Substraction() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Substract(right, scope);
        }

        public override string ToDebugString() {
            return "substraction";
        }
    }
}
