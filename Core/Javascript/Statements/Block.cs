using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Block : Statement {
        public StatementList StatementList;

        public Block(StatementList statementList) {
            StatementList = statementList;
        }

        public void BlockDeclarationInstantiation(LexicalEnvironment lex, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-blockdeclarationinstantiation

            var envRec = lex.Record;

            foreach (var statement in StatementList.List) {
                if (statement is VariableDeclaration d && d.Scope == DeclarationScope.Block) {
                    foreach (var dn in d.GetBoundNames()) {
                        if (d.IsConstant) {
                            envRec.CreateImmutableBinding(dn, true);
                        } else {
                            envRec.CreateMutableBinding(dn, false);
                        }
                    }

                    // TODO: function definitions
                }
            }
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-block-runtime-semantics-evaluation

            var oldEnv = agent.Running.Lex;
            var blockEnv = new LexicalEnvironment(oldEnv);

            BlockDeclarationInstantiation(blockEnv, agent);

            agent.Running.Lex = blockEnv;
            var r = StatementList.Evaluate(agent);
            agent.Running.Lex = oldEnv;

            return r;
        }
    }
}
