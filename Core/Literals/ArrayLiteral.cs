using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class ArrayLiteral : Literal {
        public List<Expression> Values { get; }

        public ArrayLiteral(List<Expression> values) {
            Values = values;
        }

        public override Constant Instantiate(Agent agent) {
            var array = new Array(0, agent);
            array.AddRange(Values.Select(value => value.Evaluate(agent)));
            return array;
        }

        public override string ToDebugString() {
            return "arrayblueprint";
        }
    }
}
