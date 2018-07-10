using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class ScriptRequest : Request {

        public Microsoft.ClearScript.V8.V8Script Script;

        public ScriptRequest(Microsoft.ClearScript.V8.V8Script script, JSApplication application, Action<object> resultCallback, JSSession session = null) {
            Script = script;
            State = new State(this, application, session ?? new JSSession(), new StringBuilder());
            ResultCallback = resultCallback;
        }

        public override void Call() {
            State.Set();

            try {
                State.Application.Evaluate(Script);
            } catch (Exception e) {
                State.Application.Error(e, ErrorStage.Runtime);
            }
        }
    }
}
