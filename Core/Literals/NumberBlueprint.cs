using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class NumberBlueprint : Blueprint {
        public double Value { get; }

        public NumberBlueprint(double value) {
            Value = value;
        }

        public override Constant Instantiate(Scope scope) {
            return new Number(Value);
        }

        public override string ToDebugString() {
            return "numberblueprint";
        }
    }
}
