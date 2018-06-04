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

            public override bool Start(LexicalEnvironment lex) {
                return true;
            }

            public override bool Before(LexicalEnvironment lex) {
                return _whileNode.Check.IsTrue(lex);
            }

            public override bool After(LexicalEnvironment lex) {
                return true;
            }
        }

        public override Completion Execute(LexicalEnvironment parent) {
            return new WhileExecution(this).Execute(this, parent);
        }
    }
}
