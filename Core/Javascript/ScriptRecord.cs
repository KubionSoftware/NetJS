using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class ScriptRecord {
        
        public Realm Realm;
        public LexicalEnvironment Environment;
        public Block ECMAScriptCode;

    }
}
