using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {
    public class Reference : Constant {

        private Constant _base;
        private Constant _name;
        private bool _strict;

        public Reference(Constant b, Constant name, bool strict) {
            _base = b;
            _name = name;
            _strict = strict;
        }

        public Constant GetBase() {
            return _base;
        }

        public Constant GetReferencedName() {
            return _name;
        }

        public bool IsStrictReference() {
            return _strict;
        }

        public bool HasPrimitiveBase() {
            return _base is Boolean || _base is String || _base is Symbol || _base is Number;
        }

        public bool IsPropertyReference() {
            return _base is Object || HasPrimitiveBase();
        }

        public bool IsUnresolvableReference() {
            return _base is Undefined;
        }

        public virtual bool IsSuperReference() {
            return false;
        }

        public virtual Constant GetThisValue() {
            return Static.Undefined;
        }

        public override string ToDebugString() {
            throw new NotImplementedException();
        }
    }

    class SuperReference : Reference {

        private Constant _thisValue;

        public SuperReference(Constant b, Constant name, bool strict, Constant thisValue) : base(b, name, strict) {
            _thisValue = thisValue;
        }

        public override Constant GetThisValue() {
            return _thisValue;
        }

        public override bool IsSuperReference() {
            return true;
        }
    }
}
