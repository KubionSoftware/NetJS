using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class StringBlueprint : Blueprint {
        public string Value { get; private set; }

        public StringBlueprint(string value) {
            Value = value;
        }

        public void Combine(StringBlueprint other) {
            Value += other.Value;
        }

        public override Constant Instantiate(Scope scope) {
            return new String(Value);
        }

        public override string ToDebugString() {
            return "stringblueprint";
        }
    }
}
