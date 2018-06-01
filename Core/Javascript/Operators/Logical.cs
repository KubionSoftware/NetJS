using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class LogicalNot : UnaryRightOperator {
        public LogicalNot() : base(14) { }

        public override Constant Execute(Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-logical-not-operator

            var expr = Right.Execute(scope);
            var oldValue = Convert.ToBoolean(References.GetValue(expr, scope));

            return new Boolean(!oldValue);
        }

        public override string ToDebugString() {
            return "logical not";
        }
    }

    // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-binary-logical-operators

    public class LogicalAnd : BinaryOperator {
        public LogicalAnd() : base(5) { }

        public override Constant Execute(Scope scope) {
            var lref = Left.Execute(scope);
            var lval = References.GetValue(lref, scope);
            var lbool = Convert.ToBoolean(lval);

            if (!lbool) return lval;

            var rref = Right.Execute(scope);
            return References.GetValue(rref, scope);
        }

        public override string ToDebugString() {
            return "logical and";
        }
    }

    public class LogicalOr : BinaryOperator {
        public LogicalOr() : base(4) { }

        public override Constant Execute(Scope scope) {
            var lref = Left.Execute(scope);
            var lval = References.GetValue(lref, scope);
            var lbool = Convert.ToBoolean(lval);

            if (lbool) return lval;

            var rref = Right.Execute(scope);
            return References.GetValue(rref, scope);
        }

        public override string ToDebugString() {
            return "logical or";
        }
    }
}
