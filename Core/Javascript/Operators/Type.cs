using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class TypeOf : UnaryRightOperator {
        public TypeOf() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.TypeOf(scope);
        }

        public override string ToDebugString() {
            return "typeof";
        }
    }

    public class InstanceOf : BinaryOperator {
        public InstanceOf() : base(11) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.InstanceOf(right, scope);
        }

        public override string ToDebugString() {
            return "instanceof";
        }
    }
}
