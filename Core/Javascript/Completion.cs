using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public enum CompletionType {
        Normal,
        Return,
        Break,
        Continue,
        Throw
    }

    public class Completion {
        public CompletionType Type;
        public Constant Value;
        public string Target;

        public Completion(CompletionType type) {
            Type = type;
            Value = Static.Undefined;
        }

        public Completion(CompletionType type, Constant constant) {
            Type = type;
            Value = constant;
        }

        public Completion(CompletionType type, Constant constant, string target) {
            Type = type;
            Value = constant;
            Target = target;
        }

        public static Completion UpdateEmpty(Completion completionRecord, Constant value) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-updateempty

            if (completionRecord.Value != null) return completionRecord;

            return new Completion(completionRecord.Type, value, completionRecord.Target);
        }

        public bool IsAbrupt() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-returnifabrupt

            if (Type != CompletionType.Normal) return true;
            return false;
        }
    }
}
