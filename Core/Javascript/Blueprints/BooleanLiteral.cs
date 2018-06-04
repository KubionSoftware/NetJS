using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class BooleanLiteral : Literal {
        public bool Value { get; }

        public BooleanLiteral(bool value) {
            Value = value;
        }

        public override Constant Instantiate(Agent agent) {
            return Boolean.Create(Value);
        }

        public override string ToDebugString() {
            return "booleanblueprint";
        }
    }
}
