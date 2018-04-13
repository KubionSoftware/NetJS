using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public abstract class LoopExecution {

        private const int MaxLoops = 1000;

        public Block Body;

        public LoopExecution(Block body) {
            Body = body;
        }

        public abstract bool Start(Scope scope);
        public abstract bool Before(Scope scope);
        public abstract bool After(Scope scope);

        public Result Execute(Node node, Scope parent) {
            var scope = new Scope(parent, parent, node, ScopeType.Block, parent.Buffer);
            if (!Start(scope)) return new Result(ResultType.None);

            var i = 0;
            while (true) {
                if (Before(scope)) {
                    var result = Body.Execute(scope);

                    if (result.Type == ResultType.Break) {
                        break;
                    } else if (result.Type == ResultType.Return || result.Type == ResultType.Throw) {
                        return result;
                    }

                    if (!After(scope)) {
                        break;
                    }

                    i++;
                    if (i >= MaxLoops) {
                        var message = "Maximum number of loops exceeded";
#if debug_enabled
                        throw new InternalError(Debug.Message(node, message));
#else
                        throw new InternalError(message);
#endif
                    }
                } else {
                    break;
                }
            }

            return new Result(ResultType.None);
        }
    }
}
