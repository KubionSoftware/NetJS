using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class If : Statement {

        public Expression Test;
        public Statement TrueStmt;
        public Statement FalseStmt;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-if-statement

            var exprRef = Test.Evaluate(agent);
            var exprValue = Convert.ToBoolean(References.GetValue(exprRef, agent));

            Completion stmtCompletion;
            if (exprValue) {
                stmtCompletion = TrueStmt.Evaluate(agent);
            } else if (FalseStmt != null) {
                stmtCompletion = FalseStmt.Evaluate(agent);
            } else {
                return Static.NormalCompletion;
            }

            return Completion.UpdateEmpty(stmtCompletion, Static.Undefined);
        }
    }
}
