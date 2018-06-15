using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core {

    public class Error : Exception {
#if debug_enabled
        public new List<Debug.Location> StackTrace;
#endif

        public Error(string message) : base(message) {
#if debug_enabled
            StackTrace = new List<Debug.Location>();
#endif
        }

        public void AddStackTrace(
#if debug_enabled
            Debug.Location location
#endif
        ) {
#if debug_enabled
            StackTrace.Add(location);
#endif
        }

        public override string ToString() {
            return Message + "\n" + string.Join("\n", StackTrace.Select(loc => Debug.GetFileName(loc.FileId) + " (" + loc.LineNr + ")"));
        }
    }

    public class SyntaxError : Error {
        public SyntaxError(string message) : base("Syntax error - " + message) { }
    }

    public class InternalError : Error {
        public InternalError(string message) : base("Internal error - " + message) { }
    }

    public class ReferenceError : Error {
        public ReferenceError(string message) : base("Reference error - " + message) { }
    }

    public class RangeError : Error {
        public RangeError(string message) : base("Range error - " + message) { }
    }

    public class TypeError : Error {
        public TypeError(string message) : base("Type error - " + message) { }
    }

    public class IOError : Error {
        public IOError(string message) : base("IO error - " + message) { }
    }
}