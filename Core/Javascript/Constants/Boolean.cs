using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Boolean : Constant {
        public bool Value;

        public Boolean(bool value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Boolean", scope).Get(key.ToString());
        }

        public override Constant Add(Constant other, Scope scope) {
            if (other is String s) {
                return new String((Value ? Tokens.True : Tokens.False) + s.Value);
            }

            return base.Add(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Boolean b) {
                return new Boolean(Value == b.Value);
            } else if (other is Number n) {
                return new Boolean((Value ? 1 : 0) == n.Value);
            } else if (other is String s) {
                return new Boolean((Value ? "1" : "0") == s.Value);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Boolean b) {
                return new Boolean(Value == b.Value);
            }

            return new Boolean(false);
        }

        public override Constant LogicalAnd(Constant other, Scope scope) {
            if (other is Boolean b) {
                return new Boolean(Value && b.Value);
            }

            return base.LogicalAnd(other, scope);
        }

        public override Constant LogicalOr(Constant other, Scope scope) {
            if (other is Boolean b) {
                return new Boolean(Value || b.Value);
            }

            return base.LogicalOr(other, scope);
        }

        public override Constant LogicalNot(Scope scope) {
            return new Boolean(!Value);
        }

        public override string ToString() {
            return Value ? Tokens.True : Tokens.False;
        }

        public override Constant TypeOf(Scope scope) {
            return new String("boolean");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value ? Tokens.True : Tokens.False);
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
