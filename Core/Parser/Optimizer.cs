using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class Optimizer {

        public static void Optimize(Block code) {
            // TODO: optimize

            Walker.Walk(code, statement => statement);
        }
    }
}
