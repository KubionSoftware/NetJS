using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Agent {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-agents

        public Stack<Context> Stack;
        public Context Running;

        public AgentRecord Record;

        public Agent(Realm realm) {
            var context = new Context(realm);

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
            Running = context;
        }

        public List<Debug.Frame> GetStackTrace(Debug.Location location) {
            var frames = new List<Debug.Frame>();
            frames.Add(GetFrame(1, location));

            var context = this;
            var index = 2;
            while (context.Parent != null && context.EntryNode != null) {
                var entryLocation = Debug.GetNodeLocation(context.EntryNode.Id);

                frames.Add(context.Parent.GetFrame(index, entryLocation));

                index++;
                context = context.Parent;
            }

            return frames;
        }

        public List<Debug.Scope> GetScopes() {
            var scopes = new List<Debug.Scope>();
            scopes.Add(GetScope(1));

            var lex = Lex;
            var index = 1;
            while (Lex.Outer != null) {
                lex = Lex.Outer;
                index++;

                scopes.Add(GetScope(index));
            }

            return scopes;
        }
    }
}
