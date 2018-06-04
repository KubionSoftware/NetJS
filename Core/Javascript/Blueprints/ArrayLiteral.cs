using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ArrayLiteral : Literal {
        public List<Expression> Values { get; }

        public ArrayLiteral(List<Expression> values) {
            Values = values;
        }

        public override Constant Instantiate(Agent agent) {
            var array = new Array();

            for (var i = 0; i < Values.Count; i++) {
                var blueprint = Values[i];
                array.List.Add(blueprint.Evaluate(agent));
            }

            return array;
        }

        public override string ToDebugString() {
            return "arrayblueprint";
        }
    }
}
