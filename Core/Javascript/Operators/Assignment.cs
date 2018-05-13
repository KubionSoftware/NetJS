using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Assignment : BinaryOperator {
        public Assignment() : base(2) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Assignment(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            // TODO: error if left not variable or access
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope);
            return Execute(left, right, scope);
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

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Assignment(_op.Execute(Left.Execute(scope), right, scope), scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            // TODO: error if left not variable or access
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope);
            return Execute(left, right, scope);
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }
}
