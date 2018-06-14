using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
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

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (String)obj;
            return other.Value == Value;
        }

        public override string ToDebugString() {
            return $"\"{Value}\"";
        }
    }
}
