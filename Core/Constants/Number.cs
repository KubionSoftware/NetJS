using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Number : Constant {
        public double Value;

        public Number(double value) {
            Value = value;
        }
        
        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToDebugString() {
            return ToString();
        }
    }
}
