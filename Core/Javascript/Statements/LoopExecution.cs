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
            var outerScope = new Scope(parent, parent, node, ScopeType.Block, parent.Buffer);
            if (!Start(outerScope)) return new Result(ResultType.None);

            var i = 0;
            while (true) {
                var innerScope = new Scope(parent, parent, node, ScopeType.Block, parent.Buffer);

                if (Before(innerScope)) {
                    var result = Body.Execute(innerScope);

                    if (result.Type == ResultType.Break) {
                        break;
                    } else if (result.Type == ResultType.Return || result.Type == ResultType.Throw) {
                        return result;
                    }

                    if (!After(innerScope)) {
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
