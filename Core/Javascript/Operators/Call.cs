using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Call : BinaryOperator {
        public Call() : base(16) { }

        public override Constant Execute(Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-function-calls

            var arguments = (ArgumentList)Right;
            var reference = Left.Execute(scope);
            var func = References.GetValue(reference, scope);

            Constant thisValue = Static.Undefined;

            if (reference is Reference r) {
                if(!r.IsPropertyReference() && r.GetReferencedName() is Javascript.String v && v.Value == "eval") {
                    // TODO: eval
                } else {
                    if (r.IsPropertyReference()) {
                        thisValue = r.GetThisValue();
                    } else {
                        // TODO: environment record
                    }
                }
            } else {
                thisValue = Static.Undefined;
            }

            return EvaluateDirectCall(func, thisValue, arguments, scope);
        }

        public Constant EvaluateDirectCall(Constant func, Constant thisValue, ArgumentList argumentList, Scope scope) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-evaluatedirectcall

            if (func is Function f) {
                var arguments = new Constant[argumentList.Arguments.Length];
                for(var i = 0; i < argumentList.Arguments.Length; i++) {
                    arguments[i] = argumentList.Arguments[i].Execute(scope);
                }

                return f.Call(arguments, thisValue, scope);
            } else {
                throw new TypeError("Cant' call a non-function");
            }
        }

        public override string ToDebugString() {
            return "call";
        }
    }
}
