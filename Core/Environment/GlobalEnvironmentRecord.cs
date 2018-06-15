using System;
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

        public override Completion CreateImmutableBinding(Constant name, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-createimmutablebinding-n-s

            if (DeclarativeRecord.HasBinding(name)) {
                throw new TypeError($"There is already a binding for {name}");
            }
            return DeclarativeRecord.CreateImmutableBinding(name, isStrict);
        }

        public override Completion CreateMutableBinding(Constant name, bool canBeDeleted) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-createmutablebinding-n-d

            if (DeclarativeRecord.HasBinding(name)) {
                throw new TypeError($"There is already a binding for {name}");
            }
            return DeclarativeRecord.CreateMutableBinding(name, canBeDeleted);
        }

        public override bool DeleteBinding(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-deletebinding-n

            if (DeclarativeRecord.HasBinding(name)) {
                return DeclarativeRecord.DeleteBinding(name);
            }

            var globalObject = ObjectRecord.BindingObject;
            var existingProp = globalObject.HasOwnProperty(name);
            if (existingProp) {
                var status = ObjectRecord.DeleteBinding(name);
                if (status) {
                    VarNames.Remove(name);
                }
                return status;
            }

            return true;
        }

        public override Constant GetBindingValue(Constant name, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-getbindingvalue-n-s

            if (DeclarativeRecord.HasBinding(name)) {
                return DeclarativeRecord.GetBindingValue(name, isStrict);
            }

            return ObjectRecord.GetBindingValue(name, isStrict);
        }

        public override bool HasBinding(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-hasbinding-n

            if (DeclarativeRecord.HasBinding(name)) return true;
            return ObjectRecord.HasBinding(name);
        }

        public override bool HasSuperBinding() {
            return false;
        }

        public override bool HasThisBinding() {
            return true;
        }

        public override Completion InitializeBinding(Constant name, Constant value) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-initializebinding-n-v

            if (DeclarativeRecord.HasBinding(name)) {
                return DeclarativeRecord.InitializeBinding(name, value);
            }

            return ObjectRecord.InitializeBinding(name, value);
        }

        public override Completion SetMutableBinding(Constant name, Constant value, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-global-environment-records-setmutablebinding-n-v-s

            if (DeclarativeRecord.HasBinding(name)) {
                return DeclarativeRecord.SetMutableBinding(name, value, isStrict);
            }

            return ObjectRecord.SetMutableBinding(name, value, isStrict);
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

        public bool HasLexicalDeclaration(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-haslexicaldeclaration

            return DeclarativeRecord.HasBinding(name);
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

        public Completion CreateGlobalVarBinding(Constant name, bool canBeDeleted) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-createglobalvarbinding

            var globalObject = ObjectRecord.BindingObject;
            var hasProperty = globalObject.HasOwnProperty(name);
            var extensible = globalObject.IsExtensible();

            if (!hasProperty && extensible) {
                ObjectRecord.CreateMutableBinding(name, canBeDeleted);
                ObjectRecord.InitializeBinding(name, Static.Undefined);
            }

            if (!VarNames.Contains(name)) VarNames.Add(name);
            return Static.NormalCompletion;
        }

        public Completion CreateGlobalFunctionBinding(Constant name, Constant value, bool canBeDeleted) {
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

            globalObject.Set(name, value);
            if (!VarNames.Contains(name)) VarNames.Add(name);

            return Static.NormalCompletion;
        }

        public override Dictionary<Constant, Binding> GetMap() {
            var map = new Dictionary<Constant, Binding>();
            foreach (var pair in ObjectRecord.GetMap()) map.Add(pair.Key, pair.Value);
            foreach (var pair in DeclarativeRecord.GetMap()) map.Add(pair.Key, pair.Value);
            return map;
        }

        public override string ToDebugString() {
            return "global environment record";
        }
    }
}
