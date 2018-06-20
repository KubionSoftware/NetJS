using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace NetJS.Core {

    public class LexicalEnvironment {

        public EnvironmentRecord Record;

        private LexicalEnvironment _outer;
        public LexicalEnvironment Outer {
            get {
                return _outer;
            }

            set {
                _outer = value;
            }
        }

        private LexicalEnvironment() { }

        public Debug.Scope GetScope(int index, Agent agent) {
            var localVariables = new Util.Json();

            var map = Record.GetMap(null);

            foreach (var key in map.Keys) {
                var value = Convert.ValueToJson(map[key].Value, agent);
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

                var depth = _outer != null ? _outer.Depth + 1 : 0;

                // Store the depth so it doesn't have to be calculated every time
                _depth = depth;
                return depth;
            }
        }

        public static LexicalEnvironment NewDeclarativeEnvironment(LexicalEnvironment e) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-newdeclarativeenvironment

            var env = new LexicalEnvironment();
            var envRec = new DeclarativeEnvironmentRecord();
            env.Record = envRec;
            env.Outer = e;
            return env;
        }

        public static LexicalEnvironment NewObjectEnvironment(Object o, LexicalEnvironment e) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-newobjectenvironment

            var env = new LexicalEnvironment();
            var envRec = new ObjectEnvironmentRecord(o);
            env.Record = envRec;
            env.Outer = e;
            return env;
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

        public static LexicalEnvironment NewGlobalEnvironment(Object g, Object thisValue) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-newglobalenvironment

            var env = new LexicalEnvironment();
            var objRec = new ObjectEnvironmentRecord(g);
            var dclRec = new DeclarativeEnvironmentRecord();

            var globalRec = new GlobalEnvironmentRecord();
            globalRec.ObjectRecord = objRec;
            globalRec.GlobalThisValue = thisValue;
            globalRec.DeclarativeRecord = dclRec;
            globalRec.VarNames = new HashSet<Constant>();

            env.Record = globalRec;
            env.Outer = null;
            return env;
        }
    }
}
