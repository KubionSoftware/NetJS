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

        public override Constant GetProperty(Constant key, Scope scope) {
            var keyString = key.ToString();

            // TODO: better way?
            if (keyString == "length") return new Number(Value.Length);

            return Tool.Construct("String", scope).Get(keyString);
        }
        
        public override Constant In(Constant other, Scope scope) {
            if (other is Object obj) {
                return new Boolean(obj.Has(Value));
            }

            return base.In(other, scope);
        }

        public override string ToString() {
            return Value;
        }

        public override Constant TypeOf(Scope scope) {
            return new String("string");
        }

        public override string ToDebugString() {
            return $"\"{Value}\"";
        }
    }
}
