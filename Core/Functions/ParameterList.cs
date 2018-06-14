using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public class Parameter {
        public Constant Name;
        public Type Type;

        public Parameter(Constant name, Type type) {
            Name = name;
            Type = type;
        }

        public Parameter(string name, Type type) {
            Name = new String(name);
            Type = type;
        }
    }

    public class ParameterList : Node {
        public IList<Parameter> Parameters = new List<Parameter>();

        public int ExpectedArgumentCount() {
            return Parameters.Count;
        }

        public Constant[] BoundNames() {
            var names = new Constant[Parameters.Count];
            for (var i = 0; i < Parameters.Count; i++) names[i] = Parameters[i].Name;
            return names;
        }
    }
}
