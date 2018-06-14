using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {

    public class Binding {
        public Constant Value;
        public bool IsInitialized;
        public bool IsMutable;
        public bool CanBeDeleted;
        public bool IsStrict;

        public Binding(Constant value, bool isInitialized, bool isMutable, bool canBeDeleted, bool isStrict) {
            Value = value;
            IsInitialized = isInitialized;
            IsMutable = isMutable;
            CanBeDeleted = canBeDeleted;
            IsStrict = isStrict;
        }
    }

    public class DeclarativeEnvironmentRecord : EnvironmentRecord {

        private Dictionary<Constant, Binding> _map = new Dictionary<Constant, Binding>();

        public override bool HasBinding(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-hasbinding-n
            return _map.ContainsKey(name);
        }

        public override Completion CreateMutableBinding(Constant name, bool canBeDeleted) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-createmutablebinding-n-d
            
            Assert.IsTrue(!_map.ContainsKey(name), $"{name.ToDebugString()} is already defined");
            _map.Add(name, new Binding(Static.Undefined, false, true, canBeDeleted, false));
            return Static.NormalCompletion;
        }

        public override Completion CreateImmutableBinding(Constant name, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-createimmutablebinding-n-s
            
            Assert.IsTrue(!_map.ContainsKey(name), $"{name.ToDebugString()} is already defined");
            _map.Add(name, new Binding(Static.Undefined, false, false, false, isStrict));
            return Static.NormalCompletion;
        }

        public override Completion InitializeBinding(Constant name, Constant value) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-initializebinding-n-v

            Assert.IsTrue(_map.ContainsKey(name), $"{name.ToDebugString()} is not defined");
            var binding = _map[name];
            Assert.IsTrue(!binding.IsInitialized, $"{name.ToDebugString()} is already initialized");

            binding.Value = value;
            binding.IsInitialized = true;

            return Static.NormalCompletion;
        }

        public override Completion SetMutableBinding(Constant name, Constant value, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-setmutablebinding-n-v-s

            Binding binding = null;

            if(!_map.TryGetValue(name, out binding)){
                if (isStrict) {
                    throw new ReferenceError($"Can't set {name} because it is not defined");
                }

                CreateMutableBinding(name, true);
                InitializeBinding(name, value);
                return Static.NormalCompletion;
            }

            if (binding.IsStrict) isStrict = true;

            if (!binding.IsInitialized) {
                throw new ReferenceError($"Can't set {name} because it is not initialized");
            } else if (binding.IsMutable) {
                binding.Value = value;
            } else {
                // Assert this is an attempt to change the value of an immutable binding
                if (isStrict) {
                    throw new ReferenceError($"Can't set {name} because it is immutable");
                }
            }

            return Static.NormalCompletion;
        }

        public override Constant GetBindingValue(Constant name, bool isStrict) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-getbindingvalue-n-s
            
            Assert.IsTrue(_map.ContainsKey(name), $"{name.ToDebugString()} is not defined");
            var binding = _map[name];

            if (!binding.IsInitialized) {
                throw new ReferenceError($"Can't get value of {name} because it is not initialized");
            }

            return binding.Value;
        }

        public override bool DeleteBinding(Constant name) {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-deletebinding-n
            
            Assert.IsTrue(_map.ContainsKey(name), $"{name.ToDebugString()} is not defined");
            var binding = _map[name];

            if (!binding.CanBeDeleted) return false;

            _map.Remove(name);
            return true;
        }

        public override bool HasThisBinding() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-hasthisbinding
            return false;
        }

        public override Constant GetThisBinding() {
            return Static.Undefined;
        }

        public override bool HasSuperBinding() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-hassuperbinding
            return false;
        }

        public override Constant WithBaseObject() {
            // See: https://www.ecma-international.org/ecma-262/8.0/index.html#sec-declarative-environment-records-withbaseobject
            return Static.Undefined;
        }

        public override Dictionary<Constant, Binding> GetMap() {
            return _map;
        }

        public override string ToDebugString() {
            return "environment record";
        }
    }
}
