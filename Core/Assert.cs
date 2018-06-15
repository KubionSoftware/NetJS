using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class Assert {

        public static void IsPropertyKey(Constant argument) {
            if (!(argument is String || argument is Symbol)) {
                throw new ReferenceError($"{argument.ToDebugString()} is not a property key");
            }
        }

        public static void IsCallable(Constant argument) {
            if (!(argument is Function)) {
                throw new ReferenceError($"{argument.ToDebugString()} is not a function");
            }
        }
    }
}
