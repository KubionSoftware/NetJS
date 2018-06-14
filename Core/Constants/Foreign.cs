using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Foreign : Constant {
        public object Value;

        public Foreign(object value) {
            Value = value;
        }

        public override string ToDebugString() {
            return "foreign";
        }
    }
}
