using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public abstract class Request {
        
        public State State;
        public DateTime Issued;
        public Action<object> ResultCallback;

        public Request() {
            Issued = DateTime.Now;
        }

        public int ElapsedMilliseconds() {
            return (DateTime.Now - Issued).Milliseconds;
        }

        public abstract void Call();
    }

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
            }catch (Exception e) {
                State.Application.Error(e);
            }
        }
    }

    public class FunctionRequest : Request {

        private dynamic _function;
        private object _argument0;
        private object _argument1;
        private object _argument2;
        private object _argument3;

        public FunctionRequest(dynamic function, JSApplication application, Action<object> resultCallback, JSSession session = null, object argument0 = null, object argument1 = null, object argument2 = null, object argument3 = null) {
            _function = function;
            _argument0 = argument0;
            _argument1 = argument1;
            _argument2 = argument2;
            _argument3 = argument3;
            State = new State(this, application, session ?? new JSSession(), new StringBuilder());
            ResultCallback = resultCallback;
        }

        public override void Call() {
            State.Set();

            try {
                if (_argument3 != null) {
                    _function(_argument0, _argument1, _argument2, _argument3);
                } else if (_argument2 != null) {
                    _function(_argument0, _argument1, _argument2);
                } else if (_argument1 != null) {
                    _function(_argument0, _argument1);
                } else if (_argument0 != null) {
                    _function(_argument0);
                } else {
                    _function();
                }
            }catch (Exception e) {
                State.Application.Error(e);
            }
        }
    }
}
