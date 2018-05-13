using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ParameterList : Node {
        public IList<Variable> Parameters = new List<Variable>();
    }
}
