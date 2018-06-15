using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class FunctionBlueprint : Blueprint {
        public string Name { get; set; }
        public Type Type { get; }
        public ParameterList Parameters { get; }
        public Block Body { get; set; }

        public FunctionBlueprint(string name, Type type, ParameterList parameters, Block body) {
            Name = name;
            Type = type;
            Parameters = parameters;
            Body = body;
        }

        public override Constant Instantiate(Scope scope) {
            return new InternalFunction(scope) { Name = Name, Type = Type, Parameters = Parameters, Body = Body };
        }

        public override string ToDebugString() {
            return "functionblueprint";
        }
    }
}
