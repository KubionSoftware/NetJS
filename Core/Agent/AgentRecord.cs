using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class AgentRecord {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-agents

        public bool LittleEndian;
        public bool CanBlock;
        public string Signifier;
        public bool IsLockFree1;
        public bool IsLockFree2;
        
        // TODO: candidate execution
    }
}
