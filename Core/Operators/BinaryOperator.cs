using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
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

        public override Constant Evaluate(Agent agent) {
            return Evaluate(Left.Evaluate(agent), Right.Evaluate(agent), agent);
        }

        public virtual Constant Evaluate(Constant lref, Constant rref, Agent agent) {
            throw new Exception("This operator cannot be called");
        }
    }
}
