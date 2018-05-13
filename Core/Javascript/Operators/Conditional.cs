using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Conditional : BinaryOperator {
        public Conditional() : base(3) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Conditional(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var right = Right.Execute(scope);
            Constant result = null;

            if (right is ArgumentList) {
                var list = (ArgumentList)right;
                if (list.Arguments.Length == 2) {
                    if (Left.IsTrue(scope)) {
                        result = list.Arguments[0].Execute(scope);
                    } else {
                        result = list.Arguments[1].Execute(scope);
                    }
                }
            }

            if (result == null) {
                result = Execute(Left.Execute(scope), right, scope);
            }

            return result;
        }

        public override string ToDebugString() {
            return "conditional";
        }
    }
}
