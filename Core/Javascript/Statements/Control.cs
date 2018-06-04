using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Return : Statement {
        public Expression Expression;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-return-statement

            Constant exprValue = Static.Undefined;

            if (Expression != null) {
                var exprRef = Expression.Evaluate(agent);
                exprValue = References.GetValue(exprRef, agent);
            }

            return new Completion(CompletionType.Return, exprValue);
        }
    }

    public class Break : Statement {

        public string Label;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-break-statement

            return new Completion(CompletionType.Break, null, Label);
        }
    }

    public class Continue : Statement {

        public string Label;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-continue-statement

            return new Completion(CompletionType.Continue, null, Label);
        }
    }

    public class Throw : Statement {
        public Expression Expression;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-throw-statement

            var exprRef = Expression.Evaluate(agent);
            var exprValue = References.GetValue(exprRef, agent);
            return new Completion(CompletionType.Throw, exprValue);
        }
    }
}
