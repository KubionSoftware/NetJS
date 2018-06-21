using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    class GlobalEnvironmentRecord : EnvironmentRecord {

        // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records

        public ObjectEnvironmentRecord ObjectRecord;
        public Object GlobalThisValue;
        public DeclarativeEnvironmentRecord DeclarativeRecord;
        public HashSet<Constant> VarNames;

        public override Completion CreateImmutableBinding(Constant name, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-createimmutablebinding-n-s

            if (DeclarativeRecord.HasBinding(name, agent)) {
                throw new TypeError($"There is already a binding for {name}");
            }
            return DeclarativeRecord.CreateImmutableBinding(name, isStrict, agent);
        }

        public override Completion CreateMutableBinding(Constant name, bool canBeDeleted, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-createmutablebinding-n-d

            if (DeclarativeRecord.HasBinding(name, agent)) {
                throw new TypeError($"There is already a binding for {name}");
            }
            return DeclarativeRecord.CreateMutableBinding(name, canBeDeleted, agent);
        }

        public override bool DeleteBinding(Constant name, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-deletebinding-n

            if (DeclarativeRecord.HasBinding(name, agent)) {
                return DeclarativeRecord.DeleteBinding(name, agent);
            }

            var globalObject = ObjectRecord.BindingObject;
            var existingProp = globalObject.HasOwnProperty(name);
            if (existingProp) {
                var status = ObjectRecord.DeleteBinding(name, agent);
                if (status) {
                    VarNames.Remove(name);
                }
                return status;
            }

            return true;
        }

        public override Constant GetBindingValue(Constant name, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-getbindingvalue-n-s

            if (DeclarativeRecord.HasBinding(name, agent)) {
                return DeclarativeRecord.GetBindingValue(name, isStrict, agent);
            }

            return ObjectRecord.GetBindingValue(name, isStrict, agent);
        }

        public override bool HasBinding(Constant name, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-hasbinding-n

            if (DeclarativeRecord.HasBinding(name, agent)) return true;
            return ObjectRecord.HasBinding(name, agent);
        }

        public override bool HasSuperBinding() {
            return false;
        }

        public override bool HasThisBinding() {
            return true;
        }

        public override Completion InitializeBinding(Constant name, Constant value, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-initializebinding-n-v

            if (DeclarativeRecord.HasBinding(name, agent)) {
                return DeclarativeRecord.InitializeBinding(name, value, agent);
            }

            return ObjectRecord.InitializeBinding(name, value, agent);
        }

        public override Completion SetMutableBinding(Constant name, Constant value, bool isStrict, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-setmutablebinding-n-v-s

            if (DeclarativeRecord.HasBinding(name, agent)) {
                return DeclarativeRecord.SetMutableBinding(name, value, isStrict, agent);
            }

            return ObjectRecord.SetMutableBinding(name, value, isStrict, agent);
        }

        public override Constant WithBaseObject() {
            return Static.Undefined;
        }

        public override Constant GetThisBinding() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-getthisbinding

            return GlobalThisValue;
        }

        public bool HasVarDeclaration(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-hasvardeclaration

            return VarNames.Contains(name);
        }

        public bool HasLexicalDeclaration(Constant name, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-haslexicaldeclaration

            return DeclarativeRecord.HasBinding(name, agent);
        }

        public bool HasResistrictedGlobalProperty(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-hasrestrictedglobalproperty

            var existingProp = ObjectRecord.BindingObject.GetOwnProperty(name);
            if (existingProp == null) return false;
            if (existingProp.IsConfigurable) return false;
            return true;
        }

        public bool CanDeclareGlobalVar(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-candeclareglobalvar

            var hasProperty = ObjectRecord.BindingObject.HasOwnProperty(name);
            if (hasProperty) return true;
            return ObjectRecord.BindingObject.IsExtensible();
        }

        public bool CanDeclareGlobalFunction(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-candeclareglobalfunction

            var existingProp = ObjectRecord.BindingObject.GetOwnProperty(name);
            if (existingProp == null) return ObjectRecord.BindingObject.IsExtensible();
            if (existingProp.IsConfigurable) return true;
            if (existingProp is DataProperty data && data.IsWritable && data.IsEnumerable) return true;
            return false;
        }

        public Completion CreateGlobalVarBinding(Constant name, bool canBeDeleted, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-createglobalvarbinding

            var globalObject = ObjectRecord.BindingObject;
            var hasProperty = globalObject.HasOwnProperty(name);
            var extensible = globalObject.IsExtensible();

            if (!hasProperty && extensible) {
                ObjectRecord.CreateMutableBinding(name, canBeDeleted, agent);
                ObjectRecord.InitializeBinding(name, Static.Undefined, agent);
            }

            if (!VarNames.Contains(name)) VarNames.Add(name);
            return Static.NormalCompletion;
        }

        public Completion CreateGlobalFunctionBinding(Constant name, Constant value, bool canBeDeleted, Agent agent) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-createglobalfunctionbinding

            var globalObject = ObjectRecord.BindingObject;
            var existingProp = globalObject.GetOwnProperty(name);
            Property desc;

            if (existingProp == null || existingProp.IsConfigurable) {
                desc = new DataProperty() {
                    Value = value,
                    Writable = true,
                    Enumerable = true,
                    Configurable = canBeDeleted
                };
            } else {
                desc = new DataProperty() { Value = value };
            }

            globalObject.DefinePropertyOrThrow(name, desc);

            // TODO :record that the binding for N in ObjectRecord has been initialized

            globalObject.Set(name, value, agent);
            if (!VarNames.Contains(name)) VarNames.Add(name);

            return Static.NormalCompletion;
        }

        public override ConcurrentDictionary<Constant, Binding> GetMap(Agent agent) {
            var map = new ConcurrentDictionary<Constant, Binding>();
            foreach (var pair in ObjectRecord.GetMap(agent)) map.TryAdd(pair.Key, pair.Value);
            foreach (var pair in DeclarativeRecord.GetMap(agent)) map.TryAdd(pair.Key, pair.Value);
            return map;
        }

        public override string ToDebugString() {
            return "global environment record";
        }
    }
}
