using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class ClassLiteral : Literal {
        public FunctionLiteral Constructor;
        public List<FunctionLiteral> PrototypeMethods = new List<FunctionLiteral>();
        public List<FunctionLiteral> StaticMethods = new List<FunctionLiteral>();
        public string Prototype = "Object";

        public ClassLiteral() {
            
        }

        public override Constant Instantiate(Agent agent) {
            // If there is no constructor, create a default empty function
            var constructor = Constructor == null ? Function.FunctionCreate(
                FunctionKind.Normal, 
                new ParameterList(), 
                new EmptyStatement(),
                agent.Running.Lex,
                false,
                agent
            ) : (Function)Constructor.Instantiate(agent);

            // Create the prototype and assign it to the constructor
            var prototype = Tool.Construct(Prototype, agent);
            constructor.Set("prototype", prototype, agent);
            prototype.Set("constructor", constructor, agent);

            // Assign prototype methods to prototype
            foreach(var method in PrototypeMethods) {
                prototype.Set(method.Name, method.Instantiate(agent), agent);
            }

            // Assign static methods to constructor function
            foreach (var method in StaticMethods) {
                constructor.Set(method.Name, method.Instantiate(agent), agent);
            }

            return constructor;
        }

        public override string ToDebugString() {
            return "classblueprint";
        }
    }
}
