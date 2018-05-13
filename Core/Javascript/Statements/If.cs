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
    }
}
