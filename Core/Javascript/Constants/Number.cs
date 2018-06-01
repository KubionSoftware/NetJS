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
        
        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
