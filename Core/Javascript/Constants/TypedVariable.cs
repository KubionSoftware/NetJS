using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class TypedVariable : Variable {
        public string Type { get; }

        public TypedVariable(string name, string type) : base(name) {
            Type = type;
        }

        public override Constant Assignment(Constant other, Scope scope) {
            if (!Tool.CheckType(other, Type)) {
                throw new TypeError($"Cannot assign value with type '{other.GetType()}' to static type '{Type}'");
            }

            return base.Assignment(other, scope);
        }

        public override string ToDebugString() {
            return Name + ": " + Type;
        }
    }
}
