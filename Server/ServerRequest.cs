using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Server {
    public class ServerRequest : FunctionRequest {

        public HttpContext Context;

        public ServerRequest(dynamic function, JSApplication application, Action<object> resultCallback, JSSession session, HttpContext context) 
            : base((object)function, application, resultCallback, session) 
        {
            Context = context;
        }
    }
}