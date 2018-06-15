using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class ExternalFunction : Function {
        public string Name;
        public Func<Constant, Constant[], Agent, Constant> Function;

        public ExternalFunction(
            string name, 
            Func<Constant, Constant[], Agent, Constant> function, 
            Agent agent, 
            Object prototype = null
        ) : base(prototype != null ? prototype : Tool.Prototype("Function", agent)) {
            Name = name;
            Function = function;
            Environment = agent.Running.Lex;
            Realm = agent.Running.Realm;
            FunctionKind = "normal";
            ScriptOrModule = agent.Running.ScriptOrModule;
            ThisMode = ThisMode.Lexical;
            Strict = false;
        }

        public override Constant Call(Constant thisValue, Agent agent, Constant[] arguments) {
            var callerContext = agent.Running;
            var calleeContext = PrepareForOrdinaryCall(null, agent);

            OrdinaryCallBindThis(calleeContext, thisValue, agent);
            var result = Function(thisValue, arguments, agent);

            agent.Pop();

            // TODO: return completion instead of constant
            return result;
        }

        public override string ToDebugString() {
            return $"[[{Name}]](){{}}";
        }
    }
}
