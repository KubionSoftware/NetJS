using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class File : Statement {

        private List<Expression> _expressions;

        public File(List<Expression> expressions) {
            _expressions = expressions;
        }

        public override Completion Execute(LexicalEnvironment lex) {
            foreach (var expression in _expressions) {
                lex.Buffer.Append(expression.Evaluate(lex).ToString());
            }

            return Static.NormalCompletion;
        }
    }
}
