using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class CaseClause {
        public bool IsDefault;
        public Expression Expression;
        public StatementList StatementList;

        public Constant EvaluateSelector(Agent agent) {
            var exprRef = Expression.Evaluate(agent);
            return References.GetValue(exprRef, agent);
        }

        public Completion Evaluate(Agent agent) {
            return StatementList.Evaluate(agent);
        }
    }

    public class CaseBlock {

        public List<CaseClause> Clauses;

        public Completion Evaluate(Constant input, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-runtime-semantics-caseblockevaluation

            Constant v = Static.Undefined;
            var found = false;

            foreach (var c in Clauses) {
                if (!found) {
                    if (!c.IsDefault) {
                        var clauseSelector = c.EvaluateSelector(agent);
                        found = Compare.StrictEqualityComparison(input, clauseSelector);
                    } else {
                        // TODO: handle default better
                        found = true;
                    }
                }

                if (found) {
                    var r = c.Evaluate(agent);
                    if (r.Value != null) v = r.Value;
                    if (r.IsAbrupt()) return Completion.UpdateEmpty(r, v);
                }
            }

            return Static.NormalCompletion;
        }
    }

    public class Switch : Statement {

        public Expression Test;
        public CaseBlock Block;

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-switch-statement-runtime-semantics-evaluation

            var exprRef = Test.Evaluate(agent);
            var switchValue = References.GetValue(exprRef, agent);

            var oldEnv = agent.Running.Lex;
            var blockEnv = new LexicalEnvironment(oldEnv);

            // TODO: perofm BlockDeclarationInstantiation
            agent.Running.Lex = blockEnv;
            var r = Block.Evaluate(switchValue, agent);
            agent.Running.Lex = oldEnv;

            return r;
        }
    }
}
