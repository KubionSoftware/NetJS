using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Call : BinaryOperator {
        public Call() : base(16) { }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-function-calls

            var arguments = (ArgumentList)Right;
            var reference = Left.Evaluate(agent);
            var func = References.GetValue(reference, agent);

            Constant thisValue = Static.Undefined;

            if (reference is Reference r) {
                if(!r.IsPropertyReference() && r.GetReferencedName() is String v && v.Value == "eval") {
                    // TODO: eval
                } else {
                    if (r.IsPropertyReference()) {
                        thisValue = r.GetThisValue();
                    } else if (r.GetBase() is EnvironmentRecord e) {
                        thisValue = e.WithBaseObject();
                    }
                }
            } else {
                thisValue = Static.Undefined;
            }

            return EvaluateDirectCall(func, thisValue, arguments, agent);
        }

        public Constant EvaluateDirectCall(Constant func, Constant thisValue, ArgumentList argumentList, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-evaluatedirectcall

            if (func is Function f) {
                // TODO: proper ECMAScript argument handling
                var arguments = argumentList.Evaluate(agent);

                return f.Call(thisValue, agent, arguments);
            } else {
                throw new TypeError("Cant' call a non-function");
            }
        }

        public override string ToDebugString() {
            return "call";
        }
    }
}
