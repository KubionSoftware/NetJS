using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Boolean : Constant {
        public bool Value;

        public Boolean(bool value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Boolean", scope).Get(key.ToString());
        }
        
        public override string ToString() {
            return Value ? Tokens.True : Tokens.False;
        }

        public override Constant TypeOf(Scope scope) {
            return new String("boolean");
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
