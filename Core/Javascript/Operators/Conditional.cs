using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Conditional : Operator {

        public Expression Check;
        public Expression TrueExpression;
        public Expression FalseExpression;

        public override bool HasLeft => Check != null;
        public override bool HasRight => true;

        public override void SetLeft(Expression left) {
            Check = left;
        }

        public override Expression GetLeft => Check;

        public Conditional() : base(3) { }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-conditional-operator

            var lref = Check.Evaluate(agent);
            var lval = Convert.ToBoolean(References.GetValue(lref, agent));

            if (lval) {
                var trueRef = TrueExpression.Evaluate(agent);
                return References.GetValue(trueRef, agent);
            } else {
                var falseRef = FalseExpression.Evaluate(agent);
                return References.GetValue(falseRef, agent);
            }
        }

        public override string ToDebugString() {
            return "conditional";
        }
    }
}
