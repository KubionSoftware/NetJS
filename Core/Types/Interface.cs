using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public class InterfaceBlueprint : Literal {

        private Interface _interface;

        public InterfaceBlueprint(Interface i) {
            _interface = i;
        }

        public override Constant Instantiate(Agent agent) {
            return _interface;
        }

        public override string ToDebugString() {
            throw new NotImplementedException();
        }
    }

    public class Interface : Constant {

        class Member {
            public string Name;
            public bool Optional;
            public Type Type;

            public Member(string name, bool optional, Type type) {
                Name = name;
                Optional = optional;
                Type = type;
            }

            public string ToString() {
                var optionalToken = Optional ? "?" : "";
                return $"{Name}{optionalToken}: {Type};";
            }
        }

        public string Name;
        private List<Member> _members;

        public Interface(string name) {
            Name = name;
            _members = new List<Member>();
        }

        public void Add(string name, bool optional, Type type) {
            _members.Add(new Member(name, optional, type));
        }

        public bool Check(Object o, Agent agent) {
            foreach (var member in _members) {
                var value = o.Get(member.Name);
                if (value is Undefined && member.Optional) continue;
                if (!member.Type.Check(value, agent)) return false;
            }
            return true;
        }

        public override string ToDebugString() {
            var s = "interface " + Name + "{\n";
            foreach (var member in _members) s += "\t" + member.ToString() + "\n";
            s += "}";
            return s;
        }
    }
}