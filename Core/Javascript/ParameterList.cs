using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class Parameter {
        public string Name;
        public Type Type;

        public Parameter(string name, Type type) {
            Name = name;
            Type = type;
        }
    }

    public class ParameterList : Node {
        public IList<Parameter> Parameters = new List<Parameter>();

        public int ExpectedArgumentCount() {
            return Parameters.Count;
        }
    }
}
