using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Object : Constant {
        private Fast.Dict<Constant> Properties = new Fast.Dict<Constant>(31);

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

        public override Constant Equals(Constant other, Scope scope) {
            return new Boolean(this == other);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return new Boolean(this == other);
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

        public override void Uneval(StringBuilder builder, int depth) {
            UnevalDictionary(Properties.ToDictionary(pair => pair.Key, pair => (Expression)pair.Value), builder, depth);
        }

        public static void UnevalArray(List<Expression> list, StringBuilder builder, int depth) {
            builder.Append(Tokens.ArrayOpen);

            for (var i = 0; i < list.Count; i++) {
                if (i > 0) builder.Append(", ");
                NewLine(builder, depth + 1);
                list[i].Uneval(builder, depth + 1);
            }

            NewLine(builder, depth);
            builder.Append(Tokens.ArrayClose);
        }

        public static void UnevalDictionary(Dictionary<string, Expression> dict, StringBuilder builder, int depth) {
            var keys = dict.Keys.ToArray();

            if (keys.Contains("length")) {
                var length = dict["length"];
                if (length is Number lengthNumber) {
                    if (keys.Length == lengthNumber.Value + 1) {
                        var list = new List<Expression>();
                        var success = true;

                        for (var i = 0; i < lengthNumber.Value; i++) {
                            var key = i.ToString();
                            if (!dict.ContainsKey(key)) {
                                success = false;
                                break;
                            }

                            list.Add(dict[key]);
                        }

                        if (success) {
                            UnevalArray(list, builder, depth);
                            return;
                        }
                    }
                }
            }

            builder.Append(Tokens.BlockOpen);

            for (var i = 0; i < keys.Length; i++) {
                if (i > 0) builder.Append(", ");

                NewLine(builder, depth + 1);
                builder.Append("\"" + keys[i] + "\"" + Tokens.KeyValueSeperator + " ");
                dict[keys[i]].Uneval(builder, depth + 1);
            }

            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }

        public override string ToString() {
            var builder = new StringBuilder();
            Uneval(builder, 0);
            return builder.ToString();
        }

        public override string ToDebugString() {
            return "{}";
            return $"{{\n{string.Join(",\n", Properties.Select(pair => pair.Key + ": " + pair.Value.ToDebugString()))}\n}}";
        }
    }
}
