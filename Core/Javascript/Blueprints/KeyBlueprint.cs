using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class KeyBlueprint : Blueprint {

        private string _name;

        public KeyBlueprint(string name) {
            _name = name;
        }

        public override string ToDebugString() {
            return _name;
        }

        public override Constant Instantiate(Scope scope) {
            return new String(_name);
        }
    }
}
