using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class Error : Exception {
        public Error(string message) : base(message) {

        }
    }

    public class HttpError : Error {
        public HttpError(string message) : base("HttpError - " + message) { }
    }

    public class IOError : Error {
        public IOError(string message) : base("IOError - " + message) { }
    }

    public enum ErrorStage {
        Compilation,
        Runtime
    }
}
