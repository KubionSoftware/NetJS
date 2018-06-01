using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Function : Object {
        public Scope Scope { get; }

        public Function(Scope scope) : base(Tool.Prototype("Function", scope)) {
            Scope = scope;
            Set("prototype", Tool.Construct("Object", scope));
        }

        public abstract Constant Call(Constant[] arguments, Constant thisValue, Scope scope);

        public override Constant New(Scope scope) {
            return new Constructor(this);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("function");
        }
    }
}
