using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class StatementList : Statement {

        public List<Statement> List;

        public StatementList(List<Statement> list) {
            List = list;
        }

        public override Completion Evaluate(Agent agent) {
            Constant value = Static.Undefined;
            var depth = agent.Running.Lex.Depth;
            foreach (var statement in List) {
#if debug_enabled
                if (Debug.BreakpointNodes.Contains(statement.Id)) {
                    Debug.SteppingLevel = depth;
                    Debug.Break(Debug.StopOnBreakpoint, agent.GetStackTrace(Debug.GetNodeLocation(statement.Id)), agent.GetScopes());
                } else if (Debug.SteppingInto && depth > Debug.SteppingLevel) {
                    Debug.SteppingLevel++;
                    Debug.SteppingInto = false;
                    Debug.Break(Debug.StopOnBreakpoint, agent.GetStackTrace(Debug.GetNodeLocation(statement.Id)), agent.GetScopes());
                } else if (Debug.SteppingOut && depth < Debug.SteppingLevel) {
                    Debug.SteppingLevel--;
                    Debug.SteppingOut = false;
                    Debug.Break(Debug.StopOnBreakpoint, agent.GetStackTrace(Debug.GetNodeLocation(statement.Id)), agent.GetScopes());
                } else if (Debug.SteppingOver && depth <= Debug.SteppingLevel) {
                    Debug.SteppingLevel = depth;
                    Debug.SteppingOver = false;
                    Debug.Break(Debug.StopOnBreakpoint, agent.GetStackTrace(Debug.GetNodeLocation(statement.Id)), agent.GetScopes());
                }
#endif

                Completion result;

                try {
                    result = statement.Evaluate(agent);
                } catch (Error e) {
#if debug_enabled
                    Debug.SteppingLevel = depth;
                    var location = Debug.GetNodeLocation(statement.Id);
                    Debug.Break(Debug.StopOnException, agent.GetStackTrace(location), agent.GetScopes());

                    e.AddStackTrace(location);
#endif

                    // Rethrow the error so it keeps traveling up
                    throw;
                } catch {
                    throw;
                }

                if (result.IsAbrupt()) {
#if debug_enabled
                    if (Debug.SteppingOver && depth <= 1) {
                        Debug.Continue();
                    }
#endif
                    
                    return result;
                }
                if (result.Value != null) value = result.Value;
            }
            
            return new Completion(CompletionType.Normal, value);
        }
    }
}
