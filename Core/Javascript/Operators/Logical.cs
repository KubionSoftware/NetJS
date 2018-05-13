using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class LogicalNot : UnaryRightOperator {
        public LogicalNot() : base(14) { }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var right = Right.Execute(scope);
            if (right.IsTrue(scope)) {
                return Static.False;
            } else {
                return Static.True;
            }
        }

        public override string ToDebugString() {
            return "logical not";
        }
    }

    public class LogicalAnd : BinaryOperator {
        public LogicalAnd() : base(5) { }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope);
            if (!left.IsTrue(scope)) {
                return Static.False;
            }

            var right = Right.Execute(scope);
            if (!right.IsTrue(scope)) {
                return Static.False;
            }

            return Static.True;
        }

        public override string ToDebugString() {
            return "logical and";
        }
    }

    public class LogicalOr : BinaryOperator {
        public LogicalOr() : base(4) { }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope);
            if (left.IsTrue(scope)) {
                return Static.True;
            }

            var right = Right.Execute(scope);
            if (right.IsTrue(scope)) {
                return Static.True;
            }

            return Static.False;
        }

        public override string ToDebugString() {
            return "logical or";
        }
    }
}
