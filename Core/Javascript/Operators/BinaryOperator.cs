using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class BinaryOperator : Operator {
        public Expression Left;
        public Expression Right;

        public BinaryOperator(int precedence) : base(precedence) { }

        public override bool AcceptsLeft => true;
        public override bool AcceptsRight => true;

        public override bool HasLeft => Left != null;
        public override bool HasRight => Right != null;

        public override Expression GetLeft => Left;
        public override Expression GetRight => Right;

        public override void SetLeft(Expression left) {
            Left = left;
        }

        public override void SetRight(Expression right) {
            Right = right;
        }

        public override Constant Execute(Scope scope) {
            return Execute(Left.Execute(scope), Right.Execute(scope), scope);
        }

        public virtual Constant Execute(Constant lref, Constant rref, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }
}
