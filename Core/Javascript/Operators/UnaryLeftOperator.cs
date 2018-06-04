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

        public override Constant Evaluate(Agent agent) {
            return Execute(Left.Evaluate(agent), agent);
        }

        public virtual Constant Evaluate(Constant c, Agent agent) {
            throw new Exception("This operator cannot be called");
        }
    }
}
