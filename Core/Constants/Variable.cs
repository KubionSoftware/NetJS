using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public class Variable : Constant {
        public string Name;
        public bool Constant;
        public Type Type;

        public Variable(string name, bool constant = false, Type type = null) {
            Name = name;
            Constant = constant;
            Type = type;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            if (getValue) {
                return GetValue(scope);
            } else {
                return this;
            }
        }

        public override Constant GetValue(Scope scope) {
            return scope.GetVariable(Name);
        }

        public override Constant Assignment(Constant other, Scope scope) {
            scope.SetVariable(Name, other);

            // For performance, so not everything is output as a string
            return Static.Undefined;

            //return other;
        }

        public override string ToString() {
            return Name;
        }

        public override string ToDebugString() {
            return Name;
        }
    }
}
