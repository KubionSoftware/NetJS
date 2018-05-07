using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Constant : Expression {

        public override Constant Execute(Scope scope, bool getValue = true) {
            return this;
        }

        public override Constant GetValue(Scope scope) {
            return this;
        }

        public bool IsUndefined() {
            return this is Undefined;
        }

        public string GetString(Scope scope) {
            var constant = GetValue(scope);

            if (constant is String s) {
                return s.Value;
            } else if (constant is Number n) {
                return n.Value.ToString();
            }

            return "";
        }

        public virtual T As<T>() where T : Constant {
            return (T)this;
        }

        public virtual Constant GetProperty(Constant key, Scope scope) {
            throw new ReferenceError($"Cannot access property '{key}' of {ToDebugString()}");
        }

        public virtual void SetProperty(Constant key, Constant value, Scope scope) {
            throw new ReferenceError($"Cannot assign property '{key}' of {ToDebugString()}");
        }

        private Exception OperatorException(string name, Constant a, Constant b) {
            throw new SyntaxError("Can't " + name + " " + a.ToDebugString() + " with " + b.ToDebugString());
        }

        private Exception OperatorException(string name, Constant a) {
            throw new SyntaxError("Can't " + name + " with " + a.ToDebugString());
        }

        public virtual Constant Add(Constant other, Scope scope) {
            throw OperatorException("add", this, other);
        }

        public virtual Constant Substract(Constant other, Scope scope) {
            throw OperatorException("substract", this, other);
        }

        public virtual Constant Multiply(Constant other, Scope scope) {
            throw OperatorException("multiply", this, other);
        }

        public virtual Constant Divide(Constant other, Scope scope) {
            throw OperatorException("divide", this, other);
        }

        public virtual Constant Call(Constant other, Constant _this, Scope scope) {
            throw OperatorException("call", this, other);
        }

        public virtual Constant Access(Constant other, Scope scope) {
            var path = new Path();
            path.Parts.Add(this);

            if (other is Variable || other is Number || other is String) {
                path.Parts.Add(other);
            } else {
                return Static.Undefined;
            }

            return path;
        }

        public virtual Constant Assignment(Constant other, Scope scope) {
            throw OperatorException("assign", this, other);
        }

        public virtual Constant New(Scope scope) {
            throw OperatorException("new", this);
        }

        public virtual Constant TypeOf(Scope scope) {
            return new String("undefined");
        }

        public virtual Constant InstanceOf(Constant other, Scope scope) {
            throw OperatorException("instanceof", this, other);
        }

        public virtual Constant Void(Scope scope) {
            return Static.Undefined;
        }

        public virtual Constant Delete(Scope scope) {
            return new Boolean(false);
        }

        public virtual Constant LessThan(Constant other, Scope scope) {
            throw OperatorException("< compare", this, other);
        }

        public virtual Constant LessThanEquals(Constant other, Scope scope) {
            throw OperatorException("<= compare", this, other);
        }

        public virtual Constant GreaterThan(Constant other, Scope scope) {
            throw OperatorException("> compare", this, other);
        }

        public virtual Constant GreaterThanEquals(Constant other, Scope scope) {
            throw OperatorException(">= compare", this, other);
        }

        public virtual Constant In(Constant other, Scope scope) {
            throw OperatorException("in", this, other);
        }

        public virtual Constant Conditional(Constant other, Scope scope) {
            throw OperatorException("conditional", this, other);
        }

        public virtual Constant BitwiseNot(Scope scope) {
            throw OperatorException("bitwise not", this);
        }

        public virtual Constant Negation(Scope scope) {
            throw OperatorException("negation", this);
        }

        public virtual Constant Remainder(Constant other, Scope scope) {
            throw OperatorException("remainder", this, other);
        }

        public virtual Constant LeftShift(Constant other, Scope scope) {
            throw OperatorException("left shift", this, other);
        }

        public virtual Constant RightShift(Constant other, Scope scope) {
            throw OperatorException("right shift", this, other);
        }

        public virtual Constant Equals(Constant other, Scope scope) {
            return new Boolean(false);
        }

        public virtual Constant NotEquals(Constant other, Scope scope) {
            return new Boolean(!((Boolean)Equals(other, scope)).Value);
        }

        public virtual Constant StrictEquals(Constant other, Scope scope) {
            return new Boolean(false);
        }

        public virtual Constant StrictNotEquals(Constant other, Scope scope) {
            return new Boolean(!((Boolean)StrictEquals(other, scope)).Value);
        }

        public virtual Constant BitwiseAnd(Constant other, Scope scope) {
            throw OperatorException("bitwise and", this, other);
        }

        public virtual Constant BitwiseXor(Constant other, Scope scope) {
            throw OperatorException("bitwise xor", this, other);
        }

        public virtual Constant BitwiseOr(Constant other, Scope scope) {
            throw OperatorException("bitwise or", this, other);
        }

        public virtual Constant Comma(Constant other, Scope scope) {
            /*  The comma operator evaluates each of its operands (from left to right) 
                and returns the value of the last operand. [see MDN] */
            return other;
        }

        public virtual Constant PostfixIncrement(Scope scope) {
            throw OperatorException("postfix increment", this);
        }

        public virtual Constant PostfixDecrement(Scope scope) {
            throw OperatorException("postfix decrement", this);
        }

        public virtual Constant PrefixIncrement(Scope scope) {
            throw OperatorException("prefix increment", this);
        }

        public virtual Constant PrefixDecrement(Scope scope) {
            throw OperatorException("prefix decrement", this);
        }
    }
}
