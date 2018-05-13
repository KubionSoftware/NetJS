using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class DoWhile : Statement {
        public Expression Check;
        public Block Body;

        class DoWhileExecution : LoopExecution {

            private DoWhile _doWhileNode;

            public DoWhileExecution(DoWhile doWhileNode) : base(doWhileNode.Body) {
                _doWhileNode = doWhileNode;
            }

            public override bool Start(Scope scope) {
                return true;
            }

            public override bool Before(Scope scope) {
                return true;
            }

            public override bool After(Scope scope) {
                return _doWhileNode.Check.IsTrue(scope);
            }
        }

        public override Result Execute(Scope parent) {
            return new DoWhileExecution(this).Execute(this, parent);
        }
    }
}
