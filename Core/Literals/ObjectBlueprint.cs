using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ObjectBlueprint : Blueprint {
        public Dictionary<string, Expression> Blueprints { get; }

        public ObjectBlueprint(Dictionary<string, Expression> blueprints) {
            Blueprints = blueprints;
        }

        public override Constant Instantiate(Scope scope) {
            var newObject = Tool.Construct("Object", scope);

            foreach (var key in Blueprints.Keys) {
                newObject.Set(key, Blueprints[key].Execute(scope));
            }

            return newObject;
        }

        public override string ToDebugString() {
            return "objectblueprint";
        }
    }
}
