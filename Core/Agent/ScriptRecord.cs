using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class ScriptRecord {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-script-records

        public Realm Realm;
        public LexicalEnvironment Environment;
        public Block ECMAScriptCode;
        public int FileId;

        public void GlobalDeclarationInstantiation(LexicalEnvironment env) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-globaldeclarationinstantiation

            var envRec = env.Record;

            // TODO: ECMAScript + functions

            Walker.Walk(ECMAScriptCode, node => {
                if (node is FunctionLiteral || node is ClassLiteral) return null;

                if (node is VariableDeclaration d && d.Scope != DeclarationScope.Block) {
                    foreach (var dn in d.GetBoundNames()) {
                        if (d.IsConstant) {
                            envRec.CreateImmutableBinding(dn, true);
                        } else {
                            envRec.CreateMutableBinding(dn, false);
                        }
                        envRec.InitializeBinding(dn, Static.Undefined);
                    }
                }

                return node;
            });
        }

        public Completion Evaluate(Agent agent, bool newContext = true, StringBuilder buffer = null, Object arguments = null) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-runtime-semantics-scriptevaluation
            
            Context scriptCtx;
            LexicalEnvironment scriptEnv;

            var oldBuffer = agent.Running.Buffer;
            if (buffer == null) buffer = new StringBuilder();

            if (newContext) {
                scriptCtx = new Context(Realm, buffer);
                scriptCtx.ScriptOrModule = this;

                // Deviating from ECMAScript because every file has its own lexical environment
                var globalEnv = Realm.GlobalEnv;
                scriptEnv = LexicalEnvironment.NewDeclarativeEnvironment(globalEnv);
                scriptCtx.Var = scriptEnv;
                scriptCtx.Lex = scriptEnv;

                agent.Push(scriptCtx);
            } else {
                scriptCtx = agent.Running;
                scriptCtx.Buffer = buffer;
                scriptEnv = scriptCtx.Lex;
            }

            GlobalDeclarationInstantiation(scriptEnv);

            if (arguments != null) {
                foreach (var key in arguments.OwnPropertyKeys()) {
                    scriptEnv.Record.CreateMutableBinding(key, true);
                    scriptEnv.Record.InitializeBinding(key, arguments.Get(key));
                }
            }

            var result = ECMAScriptCode.Evaluate(agent);

            if (newContext) {
                agent.Pop();
            } else {
                agent.Running.Buffer = oldBuffer;
            }

            return result;
        }

        public static ScriptRecord ParseScript(string sourceText, Realm realm, int fileId) {
            var tokens = new Lexer(sourceText, fileId).Lex();
            var parser = new Parser(fileId, tokens);
            var body = parser.Parse();

            return new ScriptRecord() {
                Realm = realm,
                Environment = null,
                ECMAScriptCode = body,
                FileId = fileId
            };
        }
    }
}
