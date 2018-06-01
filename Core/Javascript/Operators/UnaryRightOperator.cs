using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class UnaryRightOperator : Operator {
        public Expression Right;

        public UnaryRightOperator(int precedence) : base(precedence) { }

        public override bool AcceptsRight => true;
        public override bool HasRight => Right != null;
        public override Expression GetRight => Right;

        public override void SetRight(Expression right) {
            Right = right;
        }

        public override Constant Execute(Scope scope) {
            return Execute(Right.Execute(scope), scope);
        }

        public virtual Constant Execute(Constant c, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }
}
