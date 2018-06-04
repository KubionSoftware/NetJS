using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class FunctionDeclaration : Statement {

        public string Name;
        public ParameterList Parameters;
        public Block Body;

        public override Completion Evaluate(Agent agent) {
            return Static.NormalCompletion;
        }
    }
}
