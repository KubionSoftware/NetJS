using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class TimeOut {

        private DateTime _issued;
        private int _time;
        private DateTime _trigger;

        private dynamic _function;

        private State _state;

        public TimeOut(int time, dynamic function, State state) {
            _issued = DateTime.Now;
            _time = time;
            _trigger = DateTime.Now + TimeSpan.FromMilliseconds(time);

            _function = function;
            _state = state;
        }

        public bool ShouldTrigger(DateTime now) {
            return now >= _trigger;
        }

        public void Call() {
            _state.Set();
            try {
                _function();
            } catch (Exception e) {
                State.Application.Error(e);
            }
        }
    }
}
