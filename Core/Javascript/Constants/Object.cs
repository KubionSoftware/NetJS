using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Object : Constant {
        private Fast.Dict<Constant> Properties = new Fast.Dict<Constant>(16);

        public Object __proto__;

        public Object(Object proto) {
            __proto__ = proto;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Get(key.ToString());
        }

        public override void SetProperty(Constant key, Constant value, Scope scope) {
            Set(key.ToString(), value);
        }

        public Constant Get(string name) {
            Constant value = null;
            if (Properties.TryGetValue(name, ref value)) {
                return value;
            }

            if (__proto__ != null) {
                return __proto__.Get(name);
            }

            return Static.Undefined;
        }

        public T Get<T>(string name) where T : Constant {
            return (T)Get(name);
        }

        public void Set(string name, Constant value) {
            Properties.Set(name, value);
        }

        public bool Has(string name) {
            return Properties.ContainsKey(name);
        }

        public void Remove(string name) {
            Properties.Remove(name);
        }

        public string[] GetKeys() {
            return Properties.Keys.ToArray();
        }
        
        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override Constant InstanceOf(Constant other, Scope scope) {
            if(other is Object o) {
                var prototype = o.Get("prototype");
                if (__proto__ == prototype) {
                    return Static.True;
                } else if(__proto__ != null) {
                    return __proto__.InstanceOf(other, scope);
                }
            }

            return Static.False;
        }

        public override string ToDebugString() {
            return "{}";

            // TODO: fix this
            //return $"{{\n{string.Join(",\n", Properties.Select(pair => pair.Key + ": " + pair.Value.ToDebugString()))}\n}}";
        }
    }
}
