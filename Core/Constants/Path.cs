using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Path : Constant {
        public List<Constant> Parts = new List<Constant>();

        public override Constant Execute(Scope scope, bool getValue = true) {
            if (getValue) {
                return GetValue(scope);
            } else {
                return this;
            }
        }

        public override Constant GetValue(Scope scope) {
            return Get(0, scope);
        }

        public Constant GetThis(Scope scope) {
            return Get(1, scope);
        }

        public Constant Get(int offset, Scope scope) {
            Constant current = Parts[0];
            if (current is Variable currentVariable) {
                current = scope.GetVariable(currentVariable.Name);
            }

            for (var i = 1; i < Parts.Count - offset; i++) {
                current = current.GetProperty(Parts[i], scope);
            }

            return current;
        }

        public override Constant Access(Constant other, Scope scope) {
            Parts.Add(other);
            return this;
        }

        public override Constant Assignment(Constant other, Scope scope) {
            var _this = GetThis(scope);

            var property = Parts[Parts.Count - 1];
            _this.SetProperty(property, other, scope);

            // For performance, so not everything is output as a string
            return Static.Undefined;

            //return other;
        }

        public override Constant Delete(Scope scope) {
            var _this = GetThis(scope);
            var property = Parts[Parts.Count - 1];

            if (_this is Object obj) {
                obj.Remove(property.ToString());
                return Static.True;
            }

            return Static.False;
        }

        public override string ToDebugString() {
            return string.Join(".", Parts.Select(part => part.ToDebugString()));
        }
    }
}
