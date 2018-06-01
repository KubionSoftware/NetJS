using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Assignment : BinaryOperator {
        public Assignment() : base(2) { }

        public override Constant Execute(Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-assignment-operators

            if (!(Left is ObjectBlueprint || Left is ArrayBlueprint)) {
                var lref = Left.Execute(scope);
                var rref = Right.Execute(scope);

                var rval = References.GetValue(rref, scope);

                // TODO: set function name

                References.PutValue(lref, rval, scope);
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

        public override Constant Execute(Constant lref, Constant rref, Scope scope) {
            var lval = References.GetValue(lref, scope);
            var rval = References.GetValue(rref, scope);

            var r = _op.Execute(lval, rval, scope);
            References.PutValue(lref, r, scope);

            return r;
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }
}
