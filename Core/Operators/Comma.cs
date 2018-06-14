using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Comma : BinaryOperator {
        public Comma() : base(1) { }

        public override Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            References.GetValue(lref, agent);
            return References.GetValue(rref, agent);
        }

        public override string ToDebugString() {
            return "comma";
        }
    }
}
