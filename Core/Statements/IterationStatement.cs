using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class IterationStatement : Statement {
        
        public static bool LoopContinues(Completion completion, List<string> labelSet) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-loopcontinues

            if (completion.Type == CompletionType.Normal) return true;
            if (completion.Type != CompletionType.Continue) return false;
            if (completion.Target == null) return true;
            if (labelSet.Contains(completion.Target)) return true;

            return false;
        }
    }
}
