using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Context {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-execution-contexts

        public LexicalEnvironment Lex;
        public LexicalEnvironment Var;

        public Function Function;
        public Realm Realm;
        public ScriptRecord ScriptOrModule;

        public StringBuilder Buffer;

        public bool IsStrict;

        public Context (Realm realm, StringBuilder buffer) {
            Realm = realm;
            Buffer = buffer;
        }

        public Context Clone() {
            return new Context(Realm, Buffer) {
                Lex = Lex,
                Var = Var,
                Function = Function,
                ScriptOrModule = ScriptOrModule,
                IsStrict = IsStrict
            };
        }
        
        public Debug.Frame GetFrame(int index, Debug.Location location) {
            return new Debug.Frame() {
                Index = 1,
                Name = "Frame Name",
                File = Debug.GetFileName(location.FileId),
                Line = location.LineNr
            };
        }
        
        public Object GetGlobalObject() {
            return Realm.GlobalObject;
        }

        public EnvironmentRecord GetThisEnvironment() {
            var lex = Lex;
            while (true) {
                var envRec = lex.Record;
                var exists = envRec.HasThisBinding();
                if (exists) return envRec;
                var outer = lex.Outer;
                lex = outer;
            }
        }
    }
}
