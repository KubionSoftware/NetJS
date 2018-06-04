using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core.Javascript {

    public abstract class EnvironmentRecord : Constant {

        public abstract bool HasBinding(Constant name);

        public abstract Completion CreateMutableBinding(Constant name, bool canBeDeleted);

        public abstract Completion CreateImmutableBinding(Constant name, bool isStrict);

        public abstract Completion InitializeBinding(Constant name, Constant value);

        public abstract Completion SetMutableBinding(Constant name, Constant value, bool isStrict);

        public abstract Constant GetBindingValue(Constant name, bool isStrict);

        public abstract bool DeleteBinding(Constant name);

        public abstract bool HasThisBinding();

        public abstract bool HasSuperBinding();

        public abstract Constant WithBaseObject();
    }
}
