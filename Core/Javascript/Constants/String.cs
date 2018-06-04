using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class String : Constant {
        public string Value;

        public String(string value) {
            Value = value;
        }

        public override string ToString() {
            return Value;
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public override string ToDebugString() {
            return $"\"{Value}\"";
        }
    }
}
