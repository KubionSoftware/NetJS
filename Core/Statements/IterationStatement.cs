using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class IterationStatement : Statement {

        private const int MaxSafeIterations = 1000;
        
        public static bool LoopContinues(Completion completion, List<string> labelSet, ref int count, bool isSafe) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-loopcontinues

            // Custom addition to ensure loop safety
            if (isSafe) {
                count++;

                if (count > MaxSafeIterations) {
                    throw new Error($"Exceeded max loop iterations ({MaxSafeIterations}) in safe mode");
                }
            }

            if (completion.Type == CompletionType.Normal) return true;
            if (completion.Type != CompletionType.Continue) return false;
            if (completion.Target == null) return true;
            if (labelSet.Contains(completion.Target)) return true;

            return false;
        }
    }
}
