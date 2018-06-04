using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    class ExpressionStatement : Statement {

        public Expression Expression;

        public ExpressionStatement(Expression expr) {
            Expression = expr;
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-expression-statement

            var exprRef = Expression.Evaluate(agent);
            var exprValue = References.GetValue(exprRef, agent);
            return new Completion(CompletionType.Normal, exprValue);
        }
    }
}
