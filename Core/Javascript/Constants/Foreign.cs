using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Foreign : Constant {
        public object Value;

        public Foreign(object value) {
            Value = value;
        }

        public override string ToDebugString() {
            return "foreign";
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("foreign");
        }
    }
}
