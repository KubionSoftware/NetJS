using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ForOf : Statement {
        public Node Declaration;
        public Expression Collection;
        public Block Body;

        class ForOfExecution : LoopExecution {

            private ForOf _forOfNode;
            private Array _array;
            private int _index;

            public ForOfExecution(ForOf forOfNode) : base(forOfNode.Body) {
                _forOfNode = forOfNode;
            }

            public override bool Start(LexicalEnvironment lex) {
                var array = _forOfNode.Collection.Evaluate(lex);
                if (!(array is Array)) return false;

                _array = (Array)array;
                _index = 0;

                return true;
            }

            public override bool Before(LexicalEnvironment lex) {
                if (_index >= _array.List.Count) return false;
                // TODO: better way to declare
                lex.DeclareVariable(_forOfNode.Declaration.Declarations[0], _forOfNode.Declaration.IsConstant, DeclarationScope.Block, _array.List[_index]);
                _index++;
                return true;
            }

            public override bool After(LexicalEnvironment lex) {
                return true;
            }
        }

        public override Completion Execute(LexicalEnvironment parent) {
            return new ForOfExecution(this).Execute(this, parent);
        }
    }
}
