using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Agent {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-agents

        private const int MaxDepth = 100;

        public Stack<Context> Stack;
        public Context Running;

        public AgentRecord Record;

        public bool IsSafe = false;

        public Agent(Realm realm) {
            var context = new Context(realm, new StringBuilder());

            Stack = new Stack<Context>();
            Stack.Push(context);
            Running = context;

            // TODO: random signifier in record

            Record = new AgentRecord() {
                LittleEndian = true,
                CanBlock = true,
                Signifier = "",
                IsLockFree1 = true,
                IsLockFree2 = true
            };
        }

        public void Push(Context context) {
            Stack.Push(context);

            if (Stack.Count > MaxDepth) {
                // Stackoverflow
                throw new RangeError("Maximum call stack size exceeded");
            }

            Running = context;
        }

        public Context Pop() {
            var context = Stack.Pop();
            Running = Stack.Peek();
            return context;
        }

        public List<Debug.Frame> GetStackTrace(Debug.Location location) {
            var frames = new List<Debug.Frame>();

            var contexts = Stack.ToArray();
            if (contexts.Length == 0) return frames;

            var stackIndex = 1;
            for (var i = contexts.Length - 1; i >= 0; i--) {
                frames.Add(contexts[i].GetFrame(stackIndex, location));
                stackIndex++;
            }
            
            return frames;
        }

        public List<Debug.Scope> GetScopes() {
            var scopes = new List<Debug.Scope>();
            scopes.Add(Running.Lex.GetScope(1));

            var lex = Running.Lex;
            var index = 1;
            while (lex.Outer != null) {
                lex = lex.Outer;
                index++;

                scopes.Add(lex.GetScope(index));
            }

            return scopes;
        }
    }
}
