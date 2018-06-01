using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Conditional : BinaryOperator {
        public Conditional() : base(3) { }

        public override Constant Execute(Constant lref, Constant rref, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-conditional-operator

            var lval = Convert.ToBoolean(References.GetValue(lref, scope));

            // TODO: save left and right expression instead of arguments
            var arguments = (ArgumentList)rref;

            if (lval) {
                var trueRef = arguments.Arguments[0].Execute(scope);
                return References.GetValue(trueRef, scope);
            } else {
                var falseRef = arguments.Arguments[1].Execute(scope);
                return References.GetValue(falseRef, scope);
            }
        }

        public override string ToDebugString() {
            return "conditional";
        }
    }
}
