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

        public override Constant GetProperty(Constant key, LexicalEnvironment lex) {
            return Tool.Construct("Number", lex).Get(key.ToString());
        }
        
        public override Constant In(Constant other, LexicalEnvironment lex) {
            if (other is Object obj) {
                return Boolean.Create(obj.Get(Value.ToString()) is Undefined ? false : true);
            }

            return base.In(other, lex);
        }

        public override Constant BitwiseNot(LexicalEnvironment lex) {
            return new Number(~(int)Value);
        }

        public override Constant BitwiseAnd(Constant other, LexicalEnvironment lex) {
            if (other is Number n) {
                return new Number((int)Value & (int)n.Value);
            }

            return base.BitwiseAnd(other, lex);
        }

        public override Constant BitwiseOr(Constant other, LexicalEnvironment lex) {
            if (other is Number n) {
                return new Number((int)Value | (int)n.Value);
            }

            return base.BitwiseOr(other, lex);
        }

        public override Constant BitwiseXor(Constant other, LexicalEnvironment lex) {
            if (other is Number n) {
                return new Number((int)Value ^ (int)n.Value);
            }

            return base.BitwiseXor(other, lex);
        }

        public override Constant LeftShift(Constant other, LexicalEnvironment lex) {
            if (other is Number n) {
                return new Number((int)Value << (int)n.Value);
            }

            return base.LeftShift(other, lex);
        }

        public override Constant RightShift(Constant other, LexicalEnvironment lex) {
            if (other is Number n) {
                return new Number((int)Value >> (int)n.Value);
            }

            return base.RightShift(other, lex);
        }
        
        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(LexicalEnvironment lex) {
            return new String("number");
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
