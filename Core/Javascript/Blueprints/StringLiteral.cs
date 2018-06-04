using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class StringLiteral : Literal {
        public string Value { get; private set; }

        public StringLiteral(string value) {
            Value = value;
        }

        public void Combine(StringLiteral other) {
            Value += other.Value;
        }

        public override Constant Instantiate(Agent agent) {
            return new String(Value);
        }

        public override string ToDebugString() {
            return "stringblueprint";
        }
    }
}
