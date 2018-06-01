using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ForOf : Statement {
        public Declaration Declaration;
        public Expression Collection;
        public Block Body;

        class ForOfExecution : LoopExecution {

            private ForOf _forOfNode;
            private Array _array;
            private int _index;

            public ForOfExecution(ForOf forOfNode) : base(forOfNode.Body) {
                _forOfNode = forOfNode;
            }

            public override bool Start(Scope scope) {
                var array = _forOfNode.Collection.Execute(scope);
                if (!(array is Array)) return false;

                _array = (Array)array;
                _index = 0;

                return true;
            }

            public override bool Before(Scope scope) {
                if (_index >= _array.List.Count) return false;
                // TODO: better way to declare
                scope.DeclareVariable(_forOfNode.Declaration.Declarations[0], _forOfNode.Declaration.IsConstant, DeclarationScope.Block, _array.List[_index]);
                _index++;
                return true;
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForOfExecution(this).Execute(this, parent);
        }
    }
}
