using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class FunctionLiteral : Literal {

        public string Name { get; set; }
        public FunctionKind Kind;
        public Type Type { get; }
        public ParameterList Parameters { get; }
        public Statement Body { get; set; }

        public FunctionLiteral(string name, FunctionKind kind, Type type, ParameterList parameters, Statement body) {
            Name = name;
            Type = type;
            Parameters = parameters;
            Body = body;
        }

        public override Constant Instantiate(Agent agent) {
            // TODO: strict
            return Function.FunctionCreate(Kind, Parameters, Body, agent.Running.Lex, false, agent);
        }

        public override string ToDebugString() {
            return "functionblueprint";
        }
    }
}
