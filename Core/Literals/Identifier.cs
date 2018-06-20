using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class Identifier : Literal {

        public Constant Name;

        public Identifier(Constant name) {
            Name = name;
        }

        public Identifier(string name) {
            Name = new String(name);
        }

        public override Constant Instantiate(Agent agent) {
            return References.GetIdentifierReference(agent.Running.Lex, Name, false, agent);
        }

        public override string ToDebugString() {
            return Name.ToString();
        }
    }
}
