using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class ObjectEnvironmentRecord : EnvironmentRecord {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records

        public Object BindingObject;
        public bool WithEnvironment;

        public ObjectEnvironmentRecord(Object bindingObject) {
            BindingObject = bindingObject;
        }

        public override Completion CreateImmutableBinding(Constant name, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-createimmutablebinding-n-s
            throw new NotImplementedException();
        }

        public override Completion CreateMutableBinding(Constant name, bool canBeDeleted, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-createmutablebinding-n-d

            return new Completion(CompletionType.Normal, Boolean.Create(BindingObject.DefinePropertyOrThrow(name, new DataProperty() {
                Value = Static.Undefined,
                Writable = true,
                Enumerable = true,
                Configurable = canBeDeleted
            })));
        }

        public override bool DeleteBinding(Constant name, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-deletebinding-n

            return BindingObject.Delete(name);
        }

        public override Constant GetBindingValue(Constant name, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-getbindingvalue-n-s

            var value = BindingObject.HasProperty(name);
            if (!value) {
                if (!isStrict) return Static.Undefined;
                throw new ReferenceError($"{name} is not defined");
            }
            return BindingObject.Get(name, agent);
        }

        public override bool HasBinding(Constant name, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-hasbinding-n

            var foundBinding = BindingObject.HasProperty(name);
            if (!foundBinding) return false;
            if (!WithEnvironment) return true;

            // TODO: unscopables

            return true;
        }

        public override bool HasSuperBinding() {
            return false;
        }

        public override bool HasThisBinding() {
            return false;
        }

        public override Constant GetThisBinding() {
            return Static.Undefined;
        }

        public override Completion InitializeBinding(Constant name, Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-initializebinding-n-v

            // TODO: record that the binding for n in envRec has been initialized

            return SetMutableBinding(name, value, false, agent);
        }

        public override Completion SetMutableBinding(Constant name, Constant value, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-setmutablebinding-n-v-s

            return new Completion(CompletionType.Normal, Boolean.Create(BindingObject.Set(name, value, agent)));
        }

        public override string ToDebugString() {
            return "object environment record";
        }

        public override Constant WithBaseObject() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-object-environment-records-withbaseobject

            if (WithEnvironment) return BindingObject;
            return Static.Undefined;
        }

        public override ConcurrentDictionary<Constant, Binding> GetMap(Agent agent) {
            var map = new ConcurrentDictionary<Constant, Binding>();
            foreach (var prop in BindingObject.OwnPropertyKeys()) {
                map.TryAdd(prop, new Binding(BindingObject.Get(prop, agent), true, true, true, false));
            }
            return map;
        }
    }
}
