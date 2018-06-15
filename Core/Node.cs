using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public abstract class Node {

#if debug_enabled
        public int Id = -1;

        public void RegisterDebug(Debug.Location location) {
            Id = Debug.AddNode(location);
        }
#endif
        
        public static void NewLine(StringBuilder builder, int depth) {
            builder.Append("\n");
            for (var i = 0; i < depth; i++) {
                builder.Append("\t");
            }
        }
    }
}
