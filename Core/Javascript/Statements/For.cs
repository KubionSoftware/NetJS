using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class For : Statement {
        public Declaration Declaration;
        public Expression Check;
        public Expression Action;
        public Block Body;

        class ForExecution : LoopExecution {

            private For _forNode;

            public ForExecution(For forNode) : base(forNode.Body) {
                _forNode = forNode;
            }

            public override bool Start(Scope scope) {
                _forNode.Declaration.Execute(scope);
                return true;
            }

            public override bool Before(Scope scope) {
                return _forNode.Check.IsTrue(scope);
            }

            public override bool After(Scope scope) {
                _forNode.Action.Execute(scope);
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForExecution(this).Execute(this, parent);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.For + Tokens.GroupOpen);
            Declaration.Uneval(builder, depth);
            builder.Append(Tokens.ExpressionEnd + " ");
            Check.Uneval(builder, depth);
            builder.Append(Tokens.ExpressionEnd + " ");
            Action.Uneval(builder, depth);
            builder.Append(Tokens.GroupClose + Tokens.BlockOpen);
            NewLine(builder, depth + 1);
            Body.Uneval(builder, depth + 1);
            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }
    }
}
