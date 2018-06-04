using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace NetJS.Core.Javascript {

    public class LexicalEnvironment {

        private const int MAX_DEPTH = 100;

        public EnvironmentRecord Record;
        public LexicalEnvironment Outer;

        public LexicalEnvironment() {
            Record = new EnvironmentRecord();
        }

        public LexicalEnvironment(LexicalEnvironment outer) {
            Record = new EnvironmentRecord();
            Outer = outer;

            if (Depth > MAX_DEPTH) {
                // Stackoverflow
                throw new RangeError("Maximum call stack size exceeded");
            }
        }

        public Debug.Scope GetScope(int index) {
            var localVariables = new Util.Json();

            var map = Record.GetMap();

            foreach (var key in map.Keys) {
                var value = Convert.ValueToJson(map[key].Value);
                localVariables.Value[key.ToString()] = value;
            }

            return new Debug.Scope() {
                Name = "Scope name " + index,
                Variables = localVariables
            };
        }

        private int _depth = -1;
        public int Depth {
            get {
                // Check if the depth is already calculated
                if (_depth != -1) return _depth;

                var depth = Outer != null ? Outer.Depth + 1 : 0;

                // Store the depth so it doesn't have to be calculated every time
                _depth = depth;
                return depth;
            }
        }

        public static LexicalEnvironment NewFunctionEnvironment(Function f, Constant newTarget) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-newfunctionenvironment

            var env = new LexicalEnvironment();
            var envRec = new FunctionEnvironmentRecord();
            envRec.FunctionObject = f;

            if (f.ThisMode == ThisMode.Lexical) {
                envRec.ThisBindingStatus = "lexical";
            } else {
                envRec.ThisBindingStatus = "uninitialized";
            }

            var home = f.HomeObject;
            envRec.HomeObject = home;
            
            envRec.NewTarget = (Object)newTarget;

            env.Record = envRec;
            env.Outer = f.Environment;
            return env;
        }
    }
}
