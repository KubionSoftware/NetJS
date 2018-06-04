using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ForIn : Statement {
        public Node Declaration;
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

            public override bool Start(LexicalEnvironment lex) {
                var objResult = _forInNode.Collection.Evaluate(lex);
                if (objResult is Array array) objResult = array.ToObject(lex);
                if (!(objResult is Object)) return false;

                _obj = (Object)objResult;
                _keys = _obj.GetKeys();
                _index = 0;

                return true;
            }

            public override bool Before(LexicalEnvironment lex) {
                if (_index >= _keys.Length) return false;
                // TODO: better way to declare
                lex.DeclareVariable(_forInNode.Declaration.Declarations[0], _forInNode.Declaration.IsConstant, DeclarationScope.Block, new String(_keys[_index]));
                _index++;
                return true;
            }

            public override bool After(LexicalEnvironment lex) {
                return true;
            }
        }

        public override Completion Execute(LexicalEnvironment parent) {
            return new ForInExecution(this).Execute(this, parent);
        }
    }
}
