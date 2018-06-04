using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Array : Constant {
        public List<Constant> List;

        private Object _arrayObject;

        public Object GetObject(LexicalEnvironment lex) {
            if (_arrayObject == null) _arrayObject = Tool.Construct("Array", lex);
            return _arrayObject;
        }

        public Array(int length = 0) {
            List = new List<Constant>(length);

            for (var i = 0; i < length; i++) {
                List.Add(Static.Undefined);
            }
        }

        public Object ToObject(LexicalEnvironment lex) {
            var obj = Tool.Construct("Object", lex);

            for (var i = 0; i < List.Count; i++) {
                obj.Set(i.ToString(), List[i]);
            }

            return obj;
        }

        public Constant Get(int index) {
            if (index >= 0 && index < List.Count) {
                return List[index];
            } else {
                return Static.Undefined;
            }
        }

        public override Constant GetProperty(Constant key, LexicalEnvironment lex) {
            if (key is Number n) {
                return Get((int)n.Value);
            }

            var keyString = key.ToString();

            int index;
            if (int.TryParse(keyString, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) {
                return Get(index);
            }

            if (keyString == "length") {
                return new Number(List.Count);
            }

            return GetObject(lex).Get(keyString);
        }

        public override Constant InstanceOf(Constant other, LexicalEnvironment lex) {
            return GetObject(lex).InstanceOf(other, lex);
        }

        public override void SetProperty(Constant key, Constant value, LexicalEnvironment lex) {
            if (key is Number n) {
                var index = (int)n.Value;
                if (index >= 0 && index < List.Count) {
                    List[index] = value;
                }
            }
        }

        public override string ToString() {
            return "[ x" + List.Count + " ]";
        }

        public override Constant TypeOf(LexicalEnvironment lex) {
            return new String("object");
        }

        public override string ToDebugString() {
            return $"[{string.Join(", ", List.Select(item => item.ToDebugString()))}]";
        }
    }
}
