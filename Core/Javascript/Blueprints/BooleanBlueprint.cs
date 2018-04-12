using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class BooleanBlueprint : Blueprint {
        public bool Value { get; }

        public BooleanBlueprint(bool value) {
            Value = value;
        }

        public override Constant Instantiate(Scope scope) {
            return new Boolean(Value);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value ? Tokens.True : Tokens.False);
        }

        public override string ToDebugString() {
            return "booleanblueprint";
        }
    }
}
