using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class InternalFunction : Function {

        public InternalFunction(Object proto) : base(proto) { }

        public override Constant Call(Constant thisArgument, Agent agent, Constant[] argumentsList = null) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-call

            if (FunctionKind == "classConstructor") {
                throw new TypeError("Can't call a class constructor");
            }

            var callerContext = agent.Running;
            var calleeContext = PrepareForOrdinaryCall(null, agent);

            OrdinaryCallBindThis(calleeContext, thisArgument, agent);
            var result = 
        }
    }
}
