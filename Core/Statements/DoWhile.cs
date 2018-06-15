using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class DoWhile : IterationStatement {
        public Expression Check;
        public Statement Body;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-do-while-statement-runtime-semantics-labelledevaluation

            // TODO: labelset
            var labelSet = new List<string>();

            Constant v = Static.Undefined;

            bool isSafe = agent.IsSafe;
            int i = 0;

            while (true) {
                var stmtResult = Body.Evaluate(agent);

                if (!LoopContinues(stmtResult, labelSet, ref i, isSafe)) {
                    return Completion.UpdateEmpty(stmtResult, v);
                }

                if (stmtResult.Value != null) {
                    v = stmtResult.Value;
                }

                var exprRef = Check.Evaluate(agent);
                var exprValue = References.GetValue(exprRef, agent);
                if (!Convert.ToBoolean(exprValue)) {
                    return new Completion(CompletionType.Normal, v);
                }
            }
        }
    }
}
