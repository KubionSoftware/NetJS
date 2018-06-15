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
                var reference = expression.Evaluate(agent);
                var value = References.GetValue(reference, agent);
                if (value is Undefined) continue;
                var str = Convert.ToString(value, agent);
                agent.Running.Buffer.Append(str);
            }

            return Static.NormalCompletion;
        }
    }
}
