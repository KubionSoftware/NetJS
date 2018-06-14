using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Block : Statement {
        public StatementList StatementList;

        public Block(StatementList statementList) {
            StatementList = statementList;
        }

        public void BlockDeclarationInstantiation(LexicalEnvironment lex, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-blockdeclarationinstantiation

            var envRec = lex.Record;

            // TODO: function definitions

            Walker.Walk(StatementList, node => {
                if (node is FunctionLiteral || node is ClassLiteral || node is IterationStatement || node is For || node is ForInOf) return null;

                if (node is VariableDeclaration d && d.Scope == DeclarationScope.Block) {
                    foreach (var dn in d.GetBoundNames()) {
                        if (d.IsConstant) {
                            envRec.CreateImmutableBinding(dn, true);
                        } else {
                            envRec.CreateMutableBinding(dn, false);
                        }
                    }
                }

                return node;
            });
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-block-runtime-semantics-evaluation

            var oldEnv = agent.Running.Lex;
            var blockEnv = LexicalEnvironment.NewDeclarativeEnvironment(oldEnv);

            BlockDeclarationInstantiation(blockEnv, agent);

            agent.Running.Lex = blockEnv;
            var r = StatementList.Evaluate(agent);
            agent.Running.Lex = oldEnv;

            return r;
        }
    }
}
