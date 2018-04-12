using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class Statement : Node {

        public virtual Result Execute(Scope scope) {
            return new Result(ResultType.None);
        }
    }
}
