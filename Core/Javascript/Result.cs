using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public enum ResultType {
        None,
        Buffer,
        Return,
        Break,
        Continue,
        Throw
    }

    public class Result {
        public ResultType Type;
        public Constant Constant;

        public Result(ResultType type) {
            Type = type;
            Constant = Static.Undefined;
        }

        public Result(ResultType type, Constant constant) {
            Type = type;
            Constant = constant;
        }
    }
}
