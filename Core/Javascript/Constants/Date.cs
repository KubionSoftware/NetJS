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

        public Object GetObject(LexicalEnvironment lex) {
            if (_dateObject == null) _dateObject = Tool.Construct("Date", lex);
            return _dateObject;
        }

        public Date(DateTime value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, LexicalEnvironment lex) {
            return GetObject(lex).Get(key.ToString());
        }

        public override Constant InstanceOf(Constant other, LexicalEnvironment lex) {
            return GetObject(lex).InstanceOf(other, lex);
        }

        public override string ToString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override Constant TypeOf(LexicalEnvironment lex) {
            return new String("object");
        }

        public override string ToDebugString() {
            return $"Date({Value.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
