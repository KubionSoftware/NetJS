using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class While : Statement {
        public Expression Check;
        public Block Body;

        class WhileExecution : LoopExecution {

            private While _whileNode;

            public WhileExecution(While whileNode) : base(whileNode.Body) {
                _whileNode = whileNode;
            }

            public override bool Start(Scope scope) {
                return true;
            }

            public override bool Before(Scope scope) {
                return _whileNode.Check.IsTrue(scope);
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new WhileExecution(this).Execute(this, parent);
        }
    }
}
