using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class FunctionEnvironmentRecord : DeclarativeEnvironmentRecord {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-function-environment-records

        public Constant ThisValue;
        public string ThisBindingStatus;    // 'lexical', 'initialized' or 'uninitialized'
        public Object FunctionObject;
        public Object HomeObject;
        public Object NewTarget;

        public Completion BindThisValue(Constant v) {
            if (ThisBindingStatus == "initialized") {
                throw new ReferenceError("The this value is already initialized");
            }
            ThisValue = v;
            ThisBindingStatus = "initialized";
            return new Completion(CompletionType.Normal, v);
        }

        public override Constant GetThisBinding() {
            return ThisValue;
        }

        public Object GetSuperBase() {
            return HomeObject;
        }

        public override bool HasThisBinding() {
            return ThisValue != null;
        }

        public override bool HasSuperBinding() {
            return HomeObject != null;
        }

        public override Constant WithBaseObject() {
            return Static.Undefined;
        }
    }
}
