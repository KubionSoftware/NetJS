using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Void : UnaryRightOperator {
        public Void() : base(14) { }

        public override Constant Evaluate(Constant expr, Agent agent) {
            References.GetValue(expr, agent);
            return Static.Undefined;
        }

        public override string ToDebugString() {
            return "void";
        }
    }
}
