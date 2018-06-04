using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Try : Statement {
        public Block TryBody;
        public Constant CatchVariable;
        public Block CatchBody;
        public Block FinallyBody;

        public Completion CatchClauseEvaluation(Constant thrownValue, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-runtime-semantics-catchclauseevaluation

            var oldEnv = agent.Running.Lex;
            var catchEnv = new LexicalEnvironment(oldEnv);
            var catchEnvRec = catchEnv.Record;

            // TODO: multiple variables
            catchEnvRec.CreateMutableBinding(CatchVariable, false);

            agent.Running.Lex = catchEnv;
            var status = References.InitializeBoundName(CatchVariable, thrownValue, catchEnv, agent);
            if (status.IsAbrupt()) {
                agent.Running.Lex = oldEnv;
                return status;
            }

            var b = CatchBody.Evaluate(agent);
            agent.Running.Lex = oldEnv;

            return b;
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-try-statement-runtime-semantics-evaluation

            Completion c;

            try {
                var b = TryBody.Evaluate(agent);
                if (b.Type == CompletionType.Throw) {
                    c = CatchClauseEvaluation(b.Value, agent);
                } else {
                    c = b;
                }
            } catch (Exception e) {
                c = CatchClauseEvaluation(new String(e.ToString()), agent);
            }

            if (FinallyBody != null) {
                var f = FinallyBody.Evaluate(agent);
                if (f.Type == CompletionType.Normal) c = f;
            }

            return Completion.UpdateEmpty(c, Static.Undefined);
        }
    }
}
