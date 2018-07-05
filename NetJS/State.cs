using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class State {

        [ThreadStatic]
        public static Request Request;
        [ThreadStatic]
        public static JSApplication Application;
        [ThreadStatic]
        public static JSSession Session;
        [ThreadStatic]
        public static StringBuilder Buffer;


        private Request _request;
        private JSApplication _application;
        private JSSession _session;
        private StringBuilder _buffer;

        public State(Request request, JSApplication application, JSSession session, StringBuilder buffer) {
            _request = request;
            _application = application;
            _session = session;
            _buffer = buffer;
        }

        public static State Get() {
            return new State(Request, Application, Session, Buffer);
        }

        public void Set() {
            Request = _request;
            Application = _application;
            Session = _session;
            Buffer = _buffer;
        }
    }
}
