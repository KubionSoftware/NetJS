using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS {
    public class NetJSAgent : Agent {

        public JSApplication Application;
        public JSSession Session;

        public NetJSAgent(Realm realm) : base(realm) { }
    }
}
