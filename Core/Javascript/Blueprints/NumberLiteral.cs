using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class NumberLiteral : Literal {
        public double Value { get; }

        public NumberLiteral(double value) {
            Value = value;
        }

        public override Constant Instantiate(Agent agent) {
            return new Number(Value);
        }

        public override string ToDebugString() {
            return "numberblueprint";
        }
    }
}
