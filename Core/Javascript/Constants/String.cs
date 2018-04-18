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

        public override Constant Add(Constant other, Scope scope) {
            if (other is String s) {
                return new String(Value + s.Value);
            } else if (other is Number n) {
                return new String(Value + n.Value.ToString(CultureInfo.InvariantCulture));
            } else if (other is Boolean b) {
                return new String(Value + (b.Value ? Tokens.True : Tokens.False));
            } else if (other is Undefined || other is Null) {
                return this;
            }

            return base.Add(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is String s) {
                return new Boolean(Value == s.Value);
            } else if (other is Number n) {
                return new Boolean(Value == n.Value.ToString(CultureInfo.InvariantCulture));
            } else if (other is Boolean b) {
                return new Boolean(Value == (b.Value ? "1" : "0"));
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is String s) {
                return new Boolean(Value == s.Value);
            }

            return new Boolean(false);
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

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append($"\"{Value}\"");
        }

        public override string ToDebugString() {
            return $"\"{Value}\"";
        }
    }
}
