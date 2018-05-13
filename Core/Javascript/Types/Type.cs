
namespace NetJS.Core.Javascript {
    public abstract class Type {
        public abstract bool Check(Constant constant, Scope scope);
    }

    class AnyType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return true;
        }

        public override string ToString() {
            return Tokens.Any;
        }
    }

    class VoidType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return constant is Undefined;
        }

        public override string ToString() {
            return Tokens.Void;
        }
    }

    class StringType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return constant is String;
        }

        public override string ToString() {
            return Tokens.String;
        }
    }

    class NumberType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return constant is Number;
        }

        public override string ToString() {
            return Tokens.Number;
        }
    }

    class BooleanType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return constant is Boolean;
        }

        public override string ToString() {
            return Tokens.Boolean;
        }
    }

    class ObjectType : Type {
        public override bool Check(Constant constant, Scope scope) {
            return constant is Object;
        }

        public override string ToString() {
            return Tokens.Object;
        }
    }

    class ArrayType : Type {
        private Type _elementType;

        public ArrayType(Type elementType) {
            _elementType = elementType;
        }

        public override bool Check(Constant constant, Scope scope) {
            if (constant is Array array) {
                for (var i = 0; i < array.List.Count; i++) {
                    if (!_elementType.Check(array.List[i], scope)) return false;
                }
                return true;
            }

            return false;
        }

        public override string ToString() {
            return _elementType.ToString() + Tokens.Array;
        }
    }

    class InstanceType : Type {
        private Variable _instance;

        public InstanceType(Variable instance) {
            _instance = instance;
        }

        public override bool Check(Constant constant, Scope scope) {
            var value = _instance.Execute(scope);

            if(value is Interface i) {
                if (constant is Object o) {
                    return i.Check(o, scope);
                }
            } else {
                var result = new InstanceOf() { Left = constant, Right = value }.Execute(scope);
                if (result is Boolean b) {
                    return b.Value;
                }
            }
            
            return false;
        }

        public override string ToString() {
            return _instance.ToString();
        }
    }
}
