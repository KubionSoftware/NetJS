using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class UndefinedLiteral : Literal {

        public override Constant Instantiate(Agent agent) {
            return Static.Undefined;
        }

        public override string ToDebugString() {
            return "undefined blueprint";
        }
    }

    public class NullLiteral : Literal {

        public override Constant Instantiate(Agent agent) {
            return Static.Null;
        }

        public override string ToDebugString() {
            return "null blueprint";
        }
    }

    public class NaNLiteral : Literal {

        public override Constant Instantiate(Agent agent) {
            return Static.NaN;
        }

        public override string ToDebugString() {
            return "NaN blueprint";
        }
    }

    public class InfinityLiteral : Literal {

        public override Constant Instantiate(Agent agent) {
            return Static.Infinity;
        }

        public override string ToDebugString() {
            return "infinity blueprint";
        }
    }
}
