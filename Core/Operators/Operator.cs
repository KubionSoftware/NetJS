using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public abstract class Operator : Expression {
        public Operator Parent;
        public float Precedence;

        public Operator(int precedence) {
            Precedence = precedence;
        }

        public virtual bool AcceptsLeft => false;
        public virtual bool AcceptsRight => false;

        public virtual bool HasLeft => false;
        public virtual bool HasRight => false;

        public virtual void SetLeft(Expression left) { }
        public virtual void SetRight(Expression right) { }

        public virtual Expression GetLeft => null;
        public virtual Expression GetRight => null;
    }
}
