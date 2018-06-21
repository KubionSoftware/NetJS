using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public enum IterationKind {
        Enumerate,
        Iterate
    }

    public class ForInOf : IterationStatement {

        public IterationKind Kind;
        public VariableDeclaration Declaration;
        public Expression Collection;
        public Statement Body;

        public static Array GetProperties(Constant value, Agent agent) {
            // TODO: ECMAScript

            var array = new Array(0, agent);

            if(value is Array a) {
                for (var i = 0; i < a.List.Count; i++) {
                    array.Add(new String(i.ToString()), agent);
                }
            } else if (value is Object obj) {
                foreach (var prop in obj.OwnPropertyKeys()) {
                    if (prop is String) array.Add(prop, agent);
                }
            }

            return array;
        }

        public static Array GetValues(Constant value, Agent agent) {
            // TODO: ECMAScript

            var array = new Array(0, agent);

            if (value is Array a) {
                foreach (var item in a.List) {
                    array.Add(item, agent);
                }
            } else if (value is Object obj) {
                foreach (var prop in obj.OwnPropertyKeys()) {
                    if (prop is String) array.Add(prop, agent);
                }
            } 

            return array;
        }

        public Completion HeadEvaluation(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-runtime-semantics-forin-div-ofheadevaluation-tdznames-expr-iterationkind

            // TODO: TDZnames

            var oldEnv = agent.Running.Lex;

            var exprRef = Collection.Evaluate(agent);
            agent.Running.Lex = oldEnv;
            var exprValue = References.GetValue(exprRef, agent);

            if (Kind == IterationKind.Enumerate) {
                if (exprValue is Undefined || exprValue is Null) {
                    return new Completion(CompletionType.Break);
                }

                var obj = Convert.ToObject(exprValue, agent);
                return new Completion(CompletionType.Normal, GetProperties(obj, agent));
            } else {
                return new Completion(CompletionType.Normal, GetValues(exprValue, agent));
            }
        }

        public override Completion Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset

            // TODO: a lot of ECMAScript

            // TODO: labelSet
            var labelSet = new List<string>();

            bool isSafe = agent.IsSafe;
            int i = 0;

            var iterator = HeadEvaluation(agent);
            if (iterator.IsAbrupt()) return iterator;

            var oldEnv = agent.Running.Lex;

            Constant v = Static.Undefined;
            if (iterator.Value is Array array) {
                foreach (var nextValue in array.List) {
                    if (Declaration.Scope == DeclarationScope.Block) {
                        var iterationEnv = LexicalEnvironment.NewDeclarativeEnvironment(oldEnv);
                        agent.Running.Lex = iterationEnv;
                    }

                    if (Declaration.Scope == DeclarationScope.Block) {
                        var boundNames = Declaration.GetBoundNames();
                        var loopEnv = agent.Running.Lex;
                        var loopRec = loopEnv.Record;

                        foreach (var dn in boundNames) {
                            loopRec.CreateImmutableBinding(dn, true, agent);
                        }
                    }

                    var lhsRef = References.ResolveBinding(Declaration.Declarations[0].Name, null, agent);

                    if (Declaration.Scope == DeclarationScope.Block) {
                        References.InitializeReferencedBinding(lhsRef, nextValue, agent);
                    } else {
                        References.PutValue(lhsRef, nextValue, agent);
                    }

                    var result = Body.Evaluate(agent);
                    agent.Running.Lex = oldEnv;

                    if (!LoopContinues(result, labelSet, ref i, isSafe)) {
                        if (Kind == IterationKind.Enumerate) {
                            return Completion.UpdateEmpty(result, v);
                        } else {
                            // TODO: iterator close
                            return Completion.UpdateEmpty(result, v);
                        }
                    }

                    if (result.Value != null) {
                        v = result.Value;
                    }
                }
            }

            return new Completion(CompletionType.Normal, v);
        }
    }
}
