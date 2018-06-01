using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ForIn : Statement {
        public Declaration Declaration;
        public Expression Collection;
        public Block Body;

        class ForInExecution : LoopExecution {

            private ForIn _forInNode;
            private Object _obj;
            private string[] _keys;
            private int _index;

            public ForInExecution(ForIn forInNode) : base(forInNode.Body) {
                _forInNode = forInNode;
            }

            public override bool Start(Scope scope) {
                var objResult = _forInNode.Collection.Execute(scope);
                if (objResult is Array array) objResult = array.ToObject(scope);
                if (!(objResult is Object)) return false;

                _obj = (Object)objResult;
                _keys = _obj.GetKeys();
                _index = 0;

                return true;
            }

            public override bool Before(Scope scope) {
                if (_index >= _keys.Length) return false;
                // TODO: better way to declare
                scope.DeclareVariable(_forInNode.Declaration.Declarations[0], _forInNode.Declaration.IsConstant, DeclarationScope.Block, new String(_keys[_index]));
                _index++;
                return true;
            }

            public override bool After(Scope scope) {
                return true;
            }
        }

        public override Result Execute(Scope parent) {
            return new ForInExecution(this).Execute(this, parent);
        }
    }
}
