using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Array : Constant {
        public List<Constant> List;
        public Object ArrayObject;

        public Array(int length = 0) {
            List = new List<Constant>(length);

            for (var i = 0; i < length; i++) {
                List.Add(Static.Undefined);
            }
        }

        public Object ToObject(Scope scope) {
            var obj = Tool.Construct("Object", scope);

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

        public override Constant GetProperty(Constant key, Scope scope) {
            if (key is Number n) {
                return Get((int)n.Value);
            }

            var keyString = key.ToString();

            int index;
            if (int.TryParse(keyString, out index)) {
                return Get(index);
            }

            if (keyString == "length") {
                return new Number(List.Count);
            }

            if (ArrayObject == null) ArrayObject = Tool.Construct("Array", scope);
            return ArrayObject.Get(keyString);
        }

        public override void SetProperty(Constant key, Constant value, Scope scope) {
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

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Object.UnevalArray(List.Select(item => (Expression)item).ToList(), builder, depth);
        }

        public override string ToDebugString() {
            return $"[{string.Join(", ", List.Select(item => item.ToDebugString()))}]";
        }
    }
}
