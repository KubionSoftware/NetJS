using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Date : Constant {
        public DateTime Value { get; set; }

        public Date(DateTime value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Date", scope).Get(key.ToString());
        }

        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {

        }

        public override string ToDebugString() {
            return $"Date({Value.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
