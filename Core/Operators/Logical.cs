using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public class LogicalNot : UnaryRightOperator {
        public LogicalNot() : base(14) { }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-logical-not-operator

            var expr = Right.Evaluate(agent);
            var oldValue = Convert.ToBoolean(References.GetValue(expr, agent));

            return Boolean.Create(!oldValue);
        }

        public override string ToDebugString() {
            return "logical not";
        }
    }

    // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-binary-logical-operators

    public class LogicalAnd : BinaryOperator {
        public LogicalAnd() : base(5) { }

        public override Constant Evaluate(Agent agent) {
            var lref = Left.Evaluate(agent);
            var lval = References.GetValue(lref, agent);
            var lbool = Convert.ToBoolean(lval);

            if (!lbool) return lval;

            var rref = Right.Evaluate(agent);
            return References.GetValue(rref, agent);
        }

        public override string ToDebugString() {
            return "logical and";
        }
    }

    public class LogicalOr : BinaryOperator {
        public LogicalOr() : base(4) { }

        public override Constant Evaluate(Agent agent) {
            var lref = Left.Evaluate(agent);
            var lval = References.GetValue(lref, agent);
            var lbool = Convert.ToBoolean(lval);

            if (lbool) return lval;

            var rref = Right.Evaluate(agent);
            return References.GetValue(rref, agent);
        }

        public override string ToDebugString() {
            return "logical or";
        }
    }
}
