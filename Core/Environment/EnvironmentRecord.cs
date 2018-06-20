using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public abstract class EnvironmentRecord : Constant {

        public abstract bool HasBinding(Constant name, Agent agent);

        public abstract Completion CreateMutableBinding(Constant name, bool canBeDeleted, Agent agent);

        public abstract Completion CreateImmutableBinding(Constant name, bool isStrict, Agent agent);

        public abstract Completion InitializeBinding(Constant name, Constant value, Agent agent);

        public abstract Completion SetMutableBinding(Constant name, Constant value, bool isStrict, Agent agent);

        public abstract Constant GetBindingValue(Constant name, bool isStrict, Agent agent);

        public abstract bool DeleteBinding(Constant name, Agent agent);

        public abstract bool HasThisBinding();

        public abstract Constant GetThisBinding();

        public abstract bool HasSuperBinding();

        public abstract Constant WithBaseObject();

        public abstract ConcurrentDictionary<Constant, Binding> GetMap(Agent agent);
    }
}
