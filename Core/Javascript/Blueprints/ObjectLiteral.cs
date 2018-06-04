using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ObjectLiteral : Literal {
        public Dictionary<string, Expression> Blueprints { get; }

        public ObjectLiteral(Dictionary<string, Expression> blueprints) {
            Blueprints = blueprints;
        }

        public override Constant Instantiate(Agent agent) {
            var newObject = Tool.Construct("Object", agent);

            foreach (var key in Blueprints.Keys) {
                newObject.Set(new String(key), Blueprints[key].Evaluate(agent));
            }

            return newObject;
        }

        public override string ToDebugString() {
            return "objectblueprint";
        }
    }
}
