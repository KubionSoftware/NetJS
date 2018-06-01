using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Void : UnaryRightOperator {
        public Void() : base(14) { }

        public override Constant Execute(Constant expr, Scope scope) {
            References.GetValue(expr, scope);
            return Static.Undefined;
        }

        public override string ToDebugString() {
            return "void";
        }
    }
}
