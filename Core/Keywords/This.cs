using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class This : Expression {

        public override Constant Evaluate(Agent agent) {
            return References.ResolvethisBinding(agent);
        }

        public override string ToDebugString() {
            return Tokens.This;
        }
    }
}
