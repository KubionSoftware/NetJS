using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Boolean : Constant {

        public static Boolean True = Boolean.Create(true);
        public static Boolean False = Boolean.Create(false);

        public bool Value;

        private Boolean(bool value) {
            Value = value;
        }

        public static Boolean Create(bool value) {
            return value ? Static.True : Static.False;
        }
        
        public override string ToString() {
            return Value ? Tokens.True : Tokens.False;
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
