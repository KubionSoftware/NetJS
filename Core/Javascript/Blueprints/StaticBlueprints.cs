using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class UndefinedBlueprint : Blueprint {

        public override Constant Instantiate(Scope scope) {
            return Static.Undefined;
        }

        public override string ToDebugString() {
            return "undefined blueprint";
        }
    }

    public class NullBlueprint : Blueprint {

        public override Constant Instantiate(Scope scope) {
            return Static.Null;
        }

        public override string ToDebugString() {
            return "null blueprint";
        }
    }

    public class NaNBlueprint : Blueprint {

        public override Constant Instantiate(Scope scope) {
            return Static.NaN;
        }

        public override string ToDebugString() {
            return "NaN blueprint";
        }
    }

    public class InfinityBlueprint : Blueprint {

        public override Constant Instantiate(Scope scope) {
            return Static.Infinity;
        }

        public override string ToDebugString() {
            return "infinity blueprint";
        }
    }
}
