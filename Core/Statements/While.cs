using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class While : IterationStatement {
        public Expression Check;
        public Statement Body;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-while-statement-runtime-semantics-labelledevaluation

            // TODO: labelSet
            var labelSet = new List<string>();

            Constant v = Static.Undefined;

            while (true) {
                var exprRef = Check.Evaluate(agent);
                var exprValue = References.GetValue(exprRef, agent);

                if (!Convert.ToBoolean(exprValue)) {
                    return new Completion(CompletionType.Normal, v);
                }

                var stmtResult = Body.Evaluate(agent);
                if (!LoopContinues(stmtResult, labelSet)) {
                    return Completion.UpdateEmpty(stmtResult, v);
                }

                if (stmtResult.Value != null) {
                    v = stmtResult.Value;
                }
            }
        }
    }
}
