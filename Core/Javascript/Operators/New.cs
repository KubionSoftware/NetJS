using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class New : Operator {

        public Expression NewExpression;
        public ArgumentList Arguments = new ArgumentList();

        public New() : base(16) { }

        public override Constant Evaluate(Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-new-operator

            var r = NewExpression.Evaluate(agent);
            var constructor = References.GetValue(r, agent);

            var argList = Arguments.Evaluate(agent);

            if (constructor is Function f) {
                var thisValue = new Object((Object)f.Get(new String("prototype")));
                var result = f.Call(thisValue, agent, argList);
                if (result is Undefined) {
                    return thisValue;
                } else {
                    return result;
                }
            } else {
                throw new TypeError($"{constructor.ToDebugString()} is not a constructor");
            }
        }

        public override string ToDebugString() {
            return "new";
        }
    }
}
