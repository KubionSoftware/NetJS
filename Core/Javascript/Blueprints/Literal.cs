using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    abstract public class Literal : Expression {

        public abstract Constant Instantiate(Agent agent);

        public override Constant Evaluate(Agent agent) {
            return Instantiate(agent);
        }
    }
}
