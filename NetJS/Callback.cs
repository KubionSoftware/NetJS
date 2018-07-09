using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class Callback {
        private dynamic _function;
        private object _argument0;
        private object _argument1;
        private object _argument2;
        private object _argument3;

        private State _state;

        public Callback(
            dynamic function, 
            State state, 
            object argument0 = null, 
            object argument1 = null, 
            object argument2 = null, 
            object argument3 = null
        ) {
            _function = function;
            _state = state;
            _argument0 = argument0;
            _argument1 = argument1;
            _argument2 = argument2;
            _argument3 = argument3;
        }

        public void Call() {
            _state.Set();

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
            } catch (Exception e) {
                State.Application.Error(e);
            }
}
    }
}
