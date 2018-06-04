using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Assignment : BinaryOperator {
        public Assignment() : base(2) { }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-assignment-operators

            if (!(Left is ObjectLiteral || Left is ArrayLiteral)) {
                var lref = Left.Evaluate(agent);
                var rref = Right.Evaluate(agent);

                var rval = References.GetValue(rref, agent);

                // TODO: set function name

                References.PutValue(lref, rval, agent);
                return rval;
            }

            // TODO: destructuring

            return Static.Undefined;
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }

    public class AssignmentOperator : BinaryOperator {

        private BinaryOperator _op;

        public AssignmentOperator(BinaryOperator op) : base(2) {
            _op = op;
        }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            var lval = References.GetValue(lref, agent);
            var rval = References.GetValue(rref, agent);

            var r = _op.Evaluate(lval, rval, agent);
            References.PutValue(lref, r, agent);

            return r;
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }
}
