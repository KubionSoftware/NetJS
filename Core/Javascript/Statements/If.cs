using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class If : Statement {
        public class IfBlock {
            public Expression Check;
            public Block Body;

            public IfBlock(Expression check, Block body) {
                Check = check;
                Body = body;
            }
        }

        public List<IfBlock> Ifs = new List<IfBlock>();
        public Block Else;

        public override Result Execute(Scope parent) {
            foreach (var ifNode in Ifs) {
                if (ifNode.Check.IsTrue(parent)) {
                    var scope = new Scope(parent, parent, this, ScopeType.Block, parent.Buffer);
                    var result = ifNode.Body.Execute(scope);
                    return result;
                }
            }

            if (Else != null) {
                var scope = new Scope(parent, parent, this, ScopeType.Block, parent.Buffer);
                var result = Else.Execute(scope);
                return result;
            }

            return new Result(ResultType.None, Static.Undefined);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            for (var i = 0; i < Ifs.Count; i++) {
                var ifNode = Ifs[i];
                if (i > 0) builder.Append(Tokens.Else);

                builder.Append(Tokens.If + Tokens.GroupOpen);
                ifNode.Check.Uneval(builder, depth);
                builder.Append(Tokens.GroupClose + Tokens.BlockOpen);

                NewLine(builder, depth + 1);
                ifNode.Body.Uneval(builder, depth + 1);
                NewLine(builder, depth);

                builder.Append(Tokens.BlockClose);
            }

            if (Else != null) {
                builder.Append(Tokens.Else + Tokens.BlockOpen);

                NewLine(builder, depth + 1);
                Else.Uneval(builder, depth + 1);
                NewLine(builder, depth);

                builder.Append(Tokens.BlockClose);
            }
        }
    }
}
