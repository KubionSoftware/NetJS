using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Expression : Node {

        public virtual Constant Evaluate(Agent agent) {
            return Static.Undefined;
        }

        public abstract string ToDebugString();
    }
}
