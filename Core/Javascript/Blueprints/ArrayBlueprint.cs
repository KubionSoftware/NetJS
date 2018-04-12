using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ArrayBlueprint : Blueprint {
        public List<Expression> Blueprints { get; }

        public ArrayBlueprint(List<Expression> blueprints) {
            Blueprints = blueprints;
        }

        public override Constant Instantiate(Scope scope) {
            var array = new Array();

            for (var i = 0; i < Blueprints.Count; i++) {
                var blueprint = Blueprints[i];
                array.List.Add(blueprint.Execute(scope));
            }

            return array;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Object.UnevalArray(Blueprints, builder, depth);
        }

        public override string ToDebugString() {
            return "arrayblueprint";
        }
    }
}
