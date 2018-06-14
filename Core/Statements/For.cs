using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class For : IterationStatement {

        public Node First;
        public Expression Second;
        public Expression Third;

        public Statement Stmt;

        public void CreatePerIterationEnvironment(Constant[] perIterationBindings, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-createperiterationenvironment

            if (perIterationBindings.Length > 0) {
                var lastIterationEnv = agent.Running.Lex;
                var lastIterationEnvRec = lastIterationEnv.Record;
                var outer = lastIterationEnv.Outer;

                var thisIterationEnv = LexicalEnvironment.NewDeclarativeEnvironment(outer);
                var thisIterationEnvRec = thisIterationEnv.Record;

                foreach (var bn in perIterationBindings) {
                    thisIterationEnvRec.CreateMutableBinding(bn, false);
                    var lastValue = lastIterationEnvRec.GetBindingValue(bn, true);
                    thisIterationEnvRec.InitializeBinding(bn, lastValue);
                }

                agent.Running.Lex = thisIterationEnv;
            }
        }

        public Completion ForBodyEvaluation(Expression test, Expression increment, Statement stmt, Constant[] perIterationBindings, List<string> labelSet, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-forbodyevaluation

            Constant v = Static.Undefined;
            CreatePerIterationEnvironment(perIterationBindings, agent);

            while (true) {
                if(test != null) {
                    var testRef = test.Evaluate(agent);
                    var testValue = References.GetValue(testRef, agent);
                    if (!Convert.ToBoolean(testValue)) return Static.NormalCompletion;
                }

                var result = stmt.Evaluate(agent);
                if (!LoopContinues(result, labelSet)) Completion.UpdateEmpty(result, v);
                if (result.Value != null) v = result.Value;

                CreatePerIterationEnvironment(perIterationBindings, agent);
                if (increment != null) {
                    var incRef = increment.Evaluate(agent);
                    References.GetValue(incRef, agent);
                }
            }
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-for-statement-runtime-semantics-labelledevaluation

            // TODO: labelset
            var labelSet = new List<string>();

            if (First == null || First is Expression) {
                if (First is Expression fe) {
                    var exprRef = fe.Evaluate(agent);
                    References.GetValue(exprRef, agent);
                }
                return ForBodyEvaluation(Second, Third, Stmt, new Constant[0], labelSet, agent);
            }else if (First is VariableDeclaration d) {
                if (d.Scope == DeclarationScope.Block) {
                    var oldEnv = agent.Running.Lex;
                    var loopEnv = LexicalEnvironment.NewDeclarativeEnvironment(oldEnv);
                    var loopEnvRec = loopEnv.Record;
                    var isConst = d.IsConstant;
                    var boundNames = d.GetBoundNames();
                    
                    foreach (var dn in boundNames) {
                        if (isConst) {
                            loopEnvRec.CreateImmutableBinding(dn, true);
                        } else {
                            loopEnvRec.CreateMutableBinding(dn, false);
                        }
                    }

                    agent.Running.Lex = loopEnv;
                    var forDcl = d.Evaluate(agent);
                    if (forDcl.IsAbrupt()) {
                        agent.Running.Lex = oldEnv;
                        return forDcl;
                    }

                    var perIterationLets = !isConst ? boundNames : new Constant[0];
                    var bodyResult = ForBodyEvaluation(Second, Third, Stmt, perIterationLets, labelSet, agent);
                    agent.Running.Lex = oldEnv;
                    return bodyResult;
                } else {
                    var varDcl = d.Evaluate(agent);
                    if (varDcl.IsAbrupt()) return varDcl;
                    return ForBodyEvaluation(Second, Third, Stmt, new Constant[0], labelSet, agent);
                }
            }

            throw new SyntaxError("Invalid for loop");
        }
    }
}
