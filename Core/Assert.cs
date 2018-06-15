using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class Assert {

        public static void IsTrue(bool value, string message) {
            if (!value) {
                throw new Error(message);
            }
        }

        public static void IsPropertyKey(Constant argument) {
            IsTrue(argument is String || argument is Symbol, $"{argument.ToDebugString()} is not a property key");
        }

        public static void IsCallable(Constant argument) {
            IsTrue(argument is Function, $"{argument.ToDebugString()} can not be called");
        }
    }
}
