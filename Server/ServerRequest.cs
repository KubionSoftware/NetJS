using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.Server {
    public class ServerRequest : FunctionRequest {

        public HttpContext Context;

        public ServerRequest(
            dynamic function, 
            JSApplication application, 
            Action<object> resultCallback, 
            JSSession session, 
            HttpContext context,
            object argument0 = null,
            object argument1 = null,
            object argument2 = null,
            object argument3 = null
        ) 
            : base(
                  (object)function, 
                  application, 
                  resultCallback, 
                  session,
                  argument0, 
                  argument1,
                  argument2,
                  argument3
            ) 
        {
            Context = context;
        }
    }
}