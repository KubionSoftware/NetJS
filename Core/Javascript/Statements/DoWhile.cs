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

            public override bool Start(LexicalEnvironment lex) {
                return true;
            }

            public override bool Before(LexicalEnvironment lex) {
                return true;
            }

            public override bool After(LexicalEnvironment lex) {
                return _doWhileNode.Check.IsTrue(lex);
            }
        }

        public override Completion Execute(LexicalEnvironment parent) {
            return new DoWhileExecution(this).Execute(this, parent);
        }
    }
}
