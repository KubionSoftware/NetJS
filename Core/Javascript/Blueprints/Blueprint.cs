using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    abstract public class Blueprint : Expression {

        public abstract Constant Instantiate(Scope scope);

        public override Constant Execute(Scope scope) {
            return Instantiate(scope);
        }
    }
}
