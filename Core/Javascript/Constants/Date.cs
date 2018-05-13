using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Date : Constant {
        public DateTime Value { get; set; }

        private Object _dateObject;

        public Object GetObject(Scope scope) {
            if (_dateObject == null) _dateObject = Tool.Construct("Date", scope);
            return _dateObject;
        }

        public Date(DateTime value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return GetObject(scope).Get(key.ToString());
        }

        public override Constant InstanceOf(Constant other, Scope scope) {
            return GetObject(scope).InstanceOf(other, scope);
        }

        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override string ToDebugString() {
            return $"Date({Value.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
