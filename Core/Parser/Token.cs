using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Token {
        public enum Group {
            None,
            Structure,
            Operator,
            Text,
            Html,
            String,
            Template,
            Number,
            WhiteSpace,
            Comment,
            ExpressionStart,
            ExpressionEnd
        }

        public string Content;
        public Group Type;

        public int Line;
        public int Character;

        public Token(string content, Group type, int line, int character) {
            Content = content;
            Type = type;
            Line = line;
            Character = character;
        }

        public bool Is(string b) {
            return Type != Group.Comment && Type != Group.String && Type != Group.Template && Content == b;
        }

        public override string ToString() {
            return Content;
        }
    }
}
