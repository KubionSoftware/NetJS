using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class UnaryLeftOperator : Operator {
        public Expression Left;

        public UnaryLeftOperator(int precedence) : base(precedence) { }

        public override bool AcceptsLeft => true;
        public override bool HasLeft => Left != null;
        public override Expression GetLeft => Left;

        public override void SetLeft(Expression left) {
            Left = left;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope);
            return Execute(left, scope);
        }

        public virtual Constant Execute(Constant left, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }
}
