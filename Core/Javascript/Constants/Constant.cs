using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Constant {

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
        
        public virtual Constant New(Scope scope) {
            throw OperatorException("new", this);
        }

        public virtual Constant TypeOf(Scope scope) {
            return new String("undefined");
        }

        public virtual Constant InstanceOf(Constant other, Scope scope) {
            throw OperatorException("instanceof", this, other);
        }

        public virtual Constant Delete(Scope scope) {
            return new Boolean(false);
        }
        
        public virtual Constant In(Constant other, Scope scope) {
            throw OperatorException("in", this, other);
        }
        
        public virtual Constant BitwiseNot(Scope scope) {
            throw OperatorException("bitwise not", this);
        }

        public virtual Constant LeftShift(Constant other, Scope scope) {
            throw OperatorException("left shift", this, other);
        }

        public virtual Constant RightShift(Constant other, Scope scope) {
            throw OperatorException("right shift", this, other);
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

        public abstract string ToDebugString();
    }
}
