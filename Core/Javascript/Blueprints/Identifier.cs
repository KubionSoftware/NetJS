using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    class Identifier : Literal {

        public string Name;

        public Identifier(string name) {
            Name = name;
        }

        public override Constant Instantiate(LexicalEnvironment lex) {
            return LexicalEnvironment.GetIdentifierReference(lex, Name, false);
        }

        public override string ToDebugString() {
            return Name;
        }
    }
}
