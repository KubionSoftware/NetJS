using System;

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
}
