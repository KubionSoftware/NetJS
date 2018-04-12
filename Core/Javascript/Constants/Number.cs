using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Number : Constant {
        public double Value;

        public Number(double value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Number", scope).Get(key.ToString());
        }

        public override Constant Add(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number(Value + n.Value);
            } else if (other is String s) {
                return new String(Value.ToString(CultureInfo.InvariantCulture) + s.Value);
            }

            return base.Add(other, scope);
        }

        public override Constant Substract(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number(Value - n.Value);
            }

            return base.Substract(other, scope);
        }

        public override Constant Multiply(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number(Value * n.Value);
            }

            return base.Multiply(other, scope);
        }

        public override Constant Divide(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number(Value / n.Value);
            }

            return base.Divide(other, scope);
        }

        public override Constant Remainder(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number(Value % n.Value);
            }

            return base.Divide(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value == n.Value);
            } else if (other is String s) {
                return new Boolean(Value.ToString(CultureInfo.InvariantCulture) == s.Value);
            } else if (other is Boolean b) {
                return new Boolean(Value == (b.Value ? 1 : 0));
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value == n.Value);
            }

            return new Boolean(false);
        }

        public override Constant LessThan(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value < n.Value);
            }

            return base.LessThan(other, scope);
        }

        public override Constant LessThanEquals(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value <= n.Value);
            }

            return base.LessThanEquals(other, scope);
        }

        public override Constant GreaterThan(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value > n.Value);
            }

            return base.GreaterThan(other, scope);
        }

        public override Constant GreaterThanEquals(Constant other, Scope scope) {
            if (other is Number n) {
                return new Boolean(Value >= n.Value);
            }

            return base.GreaterThanEquals(other, scope);
        }

        public override Constant In(Constant other, Scope scope) {
            if (other is Object obj) {
                return new Boolean(obj.Get(Value.ToString()) is Undefined ? false : true);
            }

            return base.In(other, scope);
        }

        public override Constant BitwiseNot(Scope scope) {
            return new Number(~(int)Value);
        }

        public override Constant BitwiseAnd(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number((int)Value & (int)n.Value);
            }

            return base.BitwiseAnd(other, scope);
        }

        public override Constant BitwiseOr(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number((int)Value | (int)n.Value);
            }

            return base.BitwiseOr(other, scope);
        }

        public override Constant BitwiseXor(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number((int)Value ^ (int)n.Value);
            }

            return base.BitwiseXor(other, scope);
        }

        public override Constant LeftShift(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number((int)Value << (int)n.Value);
            }

            return base.LeftShift(other, scope);
        }

        public override Constant RightShift(Constant other, Scope scope) {
            if (other is Number n) {
                return new Number((int)Value >> (int)n.Value);
            }

            return base.RightShift(other, scope);
        }

        public override Constant Negation(Scope scope) {
            return new Number(-Value);
        }

        public override Constant PostfixIncrement(Scope scope) {
            return new Number(Value++);
        }

        public override Constant PostfixDecrement(Scope scope) {
            return new Number(Value--);
        }

        public override Constant PrefixIncrement(Scope scope) {
            return new Number(++Value);
        }

        public override Constant PrefixDecrement(Scope scope) {
            return new Number(--Value);
        }

        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value.ToString(CultureInfo.InvariantCulture));
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
