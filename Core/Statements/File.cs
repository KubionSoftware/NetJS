using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class File : Statement {

        private List<Expression> _expressions;

        public File(List<Expression> expressions) {
            _expressions = expressions;
        }

        public override Completion Evaluate(Agent agent) {
            foreach (var expression in _expressions) {
                agent.Running.Buffer.Append(expression.Evaluate(agent).ToString());
            }

            return Static.NormalCompletion;
        }
    }
}
