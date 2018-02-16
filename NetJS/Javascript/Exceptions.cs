using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Javascript {

    public class Error : Exception {
        public new List<Debug.Location> StackTrace;

        public Error(string message) : base(message) {
            StackTrace = new List<Debug.Location>();
        }

        public void AddStackTrace(Debug.Location location) {
            StackTrace.Add(location);
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
}