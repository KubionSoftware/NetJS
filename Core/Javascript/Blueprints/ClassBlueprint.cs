using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    class ClassBlueprint : Blueprint {
        public FunctionBlueprint Constructor;
        public List<FunctionBlueprint> PrototypeMethods = new List<FunctionBlueprint>();
        public List<FunctionBlueprint> StaticMethods = new List<FunctionBlueprint>();
        public string Prototype = "Object";

        public ClassBlueprint() {
            
        }

        public override Constant Instantiate(Scope scope) {
            // If there is no constructor, create a default empty function
            var constructor = Constructor == null ? new Javascript.InternalFunction(scope) {
                Name = "",
                Body = new Block() { Nodes = new List<Node>() },
                Parameters = new ParameterList() { Parameters = new List<Variable>() },
                Type = null
            } : (Function)Constructor.Instantiate(scope);

            // Create the prototype and assign it to the constructor
            var prototype = Tool.Construct(Prototype, scope);
            constructor.Set("prototype", prototype);
            prototype.Set("constructor", constructor);

            // Assign prototype methods to prototype
            foreach(var method in PrototypeMethods) {
                prototype.Set(method.Name, method.Instantiate(scope));
            }

            // Assign static methods to constructor function
            foreach (var method in StaticMethods) {
                constructor.Set(method.Name, method.Instantiate(scope));
            }

            return constructor;
        }

        public override string ToDebugString() {
            return "classblueprint";
        }
    }
}
