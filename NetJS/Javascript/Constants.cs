using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetJS.Javascript {

    public class ArgumentList : Constant {
        public IList<Expression> Arguments;

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.GroupOpen);

            for(var i = 0; i < Arguments.Count; i++) {
                if (i > 0) builder.Append(", ");
                Arguments[i].Uneval(builder, depth);
            }

            builder.Append(Tokens.GroupClose);
        }

        public override string ToDebugString() {
            var builder = new StringBuilder();
            Uneval(builder, 0);
            return builder.ToString();
        }
    }

    public class Object : Constant {
        private Fast.Dict<Constant> Properties = new Fast.Dict<Constant>(31);

        public Object __proto__;

        public Object(Object proto) {
            __proto__ = proto;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Get(key.ToString());
        }

        public override void SetProperty(Constant key, Constant value, Scope scope) {
            Set(key.ToString(), value);
        }

        public Constant Get(string name) {
            Constant value = null;
            if(Properties.TryGetValue(name, ref value)) {
                return value;
            }

            if (__proto__ != null) {
                return __proto__.Get(name);
            }

            return Static.Undefined;
        }

        public T Get<T>(string name) where T : Constant {
            return (T)Get(name);
        }

        public void Set(string name, Constant value) {
            Properties.Set(name, value);
        }

        public void Remove(string name) {
            Properties.Remove(name);
        }

        public string[] GetKeys() {
            return Properties.Keys.ToArray();
        }

        public override Constant Equals(Constant other, Scope scope) {
            return new Boolean(this == other);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            return new Boolean(this == other);
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            UnevalDictionary(Properties.ToDictionary(pair => pair.Key, pair => (Expression)pair.Value), builder, depth);
        }

        public static void UnevalArray(List<Expression> list, StringBuilder builder, int depth) {
            builder.Append(Tokens.ArrayOpen);

            for(var i = 0; i < list.Count; i++) {
                if (i > 0) builder.Append(", ");
                NewLine(builder, depth + 1);
                list[i].Uneval(builder, depth + 1);
            }

            NewLine(builder, depth);
            builder.Append(Tokens.ArrayClose);
        }

        public static void UnevalDictionary(Dictionary<string, Expression> dict, StringBuilder builder, int depth) {
            var keys = dict.Keys.ToArray();

            if (keys.Contains("length")) {
                var length = dict["length"];
                if(length is Number) {
                    var lengthNumber = (Number)length;
                    if (keys.Length == lengthNumber.Value + 1) {
                        var list = new List<Expression>();
                        var success = true;

                        for (var i = 0; i < lengthNumber.Value; i++) {
                            var key = i.ToString();
                            if (!dict.ContainsKey(key)) {
                                success = false;
                                break;
                            }

                            list.Add(dict[key]);
                        }

                        if (success) {
                            UnevalArray(list, builder, depth);
                            return;
                        }
                    }
                }
            }

            builder.Append(Tokens.BlockOpen);

            for (var i = 0; i < keys.Length; i++) {
                if (i > 0) builder.Append(", ");

                NewLine(builder, depth + 1);
                builder.Append("\"" + keys[i] + "\"" + Tokens.KeyValueSeperator + " ");
                dict[keys[i]].Uneval(builder, depth + 1);
            }

            NewLine(builder, depth);
            builder.Append(Tokens.BlockClose);
        }

        public override string ToString() {
            var builder = new StringBuilder();
            Uneval(builder, 0);
            return builder.ToString();
        }

        public override string ToDebugString() {
            return $"{{\n{string.Join(",\n", Properties.Select(pair => pair.Key + ": " + pair.Value.ToDebugString()))}\n}}";
        }
    }

    public class String : Constant {
        public string Value;

        public String(string value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            var keyString = key.ToString();

            // TODO: better way?
            if (keyString == "length") return new Number(Value.Length);

            return Tool.Construct("String", scope).Get(keyString);
        }

        public override Constant Add(Constant other, Scope scope) {
            if (other is String) {
                return new String(Value + ((String)other).Value);
            } else if (other is Number) {
                return new String(Value + ((Number)other).Value);
            } else if (other is Boolean) {
                return new String(Value + (((Boolean)other).Value ? Tokens.True : Tokens.False));
            } else if (other is Undefined || other is Null) {
                return this;
            }

            return base.Add(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if(other is String) {
                return new Boolean(Value == ((String)other).Value);
            } else if(other is Number) {
                return new Boolean(Value == ((Number)other).Value.ToString());
            } else if(other is Boolean) {
                return new Boolean(Value == (((Boolean)other).Value ? "1" : "0"));
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is String) {
                return new Boolean(Value == ((String)other).Value);
            }

            return new Boolean(false);
        }

        public override Constant In(Constant other, Scope scope) {
            if(other is Object) {
                var obj = (Object)other;
                return new Boolean(obj.Get(Value) is Undefined ? false : true);
            }

            return base.In(other, scope);
        }

        public override string ToString() {
            return Value;
        }

        public override Constant TypeOf(Scope scope) {
            return new String("string");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append($"\"{Value}\"");
        }

        public override string ToDebugString() {
            return $"\"{Value}\"";
        }
    }

    public class Number : Constant {
        public double Value;

        public Number(double value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Number", scope).Get(key.ToString());
        }

        public override Constant Add(Constant other, Scope scope) {
            if(other is Number) {
                return new Number(Value + ((Number)other).Value);
            } else if (other is String) {
                return new String(Value + ((String)other).Value);
            }

            return base.Add(other, scope);
        }

        public override Constant Substract(Constant other, Scope scope) {
            if (other is Number) {
                return new Number(Value - ((Number)other).Value);
            }

            return base.Substract(other, scope);
        }

        public override Constant Multiply(Constant other, Scope scope) {
            if (other is Number) {
                return new Number(Value * ((Number)other).Value);
            }

            return base.Multiply(other, scope);
        }

        public override Constant Divide(Constant other, Scope scope) {
            if (other is Number) {
                return new Number(Value / ((Number)other).Value);
            }

            return base.Divide(other, scope);
        }

        public override Constant Remainder(Constant other, Scope scope) {
            if (other is Number) {
                return new Number(Value % ((Number)other).Value);
            }

            return base.Divide(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value == ((Number)other).Value);
            } else if (other is String) {
                return new Boolean(Value.ToString() == ((String)other).Value);
            } else if (other is Boolean) {
                return new Boolean(Value == (((Boolean)other).Value ? 1 : 0));
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value == ((Number)other).Value);
            }

            return new Boolean(false);
        }

        public override Constant LessThan(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value < ((Number)other).Value);
            }

            return base.LessThan(other, scope);
        }

        public override Constant LessThanEquals(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value <= ((Number)other).Value);
            }

            return base.LessThanEquals(other, scope);
        }

        public override Constant GreaterThan(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value > ((Number)other).Value);
            }

            return base.GreaterThan(other, scope);
        }

        public override Constant GreaterThanEquals(Constant other, Scope scope) {
            if (other is Number) {
                return new Boolean(Value >= ((Number)other).Value);
            }

            return base.GreaterThanEquals(other, scope);
        }

        public override Constant In(Constant other, Scope scope) {
            if (other is Object) {
                var obj = (Object)other;
                return new Boolean(obj.Get(Value.ToString()) is Undefined ? false : true);
            }

            return base.In(other, scope);
        }

        public override Constant BitwiseNot(Scope scope) {
            return new Number(~(int)Value);
        }

        public override Constant BitwiseAnd(Constant other, Scope scope) {
            if (other is Number) {
                return new Number((int)Value & (int)((Number)other).Value);
            }

            return base.BitwiseAnd(other, scope);
        }

        public override Constant BitwiseOr(Constant other, Scope scope) {
            if (other is Number) {
                return new Number((int)Value | (int)((Number)other).Value);
            }

            return base.BitwiseOr(other, scope);
        }

        public override Constant BitwiseXor(Constant other, Scope scope) {
            if (other is Number) {
                return new Number((int)Value ^ (int)((Number)other).Value);
            }

            return base.BitwiseXor(other, scope);
        }

        public override Constant LeftShift(Constant other, Scope scope) {
            if (other is Number) {
                return new Number((int)Value << (int)((Number)other).Value);
            }

            return base.LeftShift(other, scope);
        }

        public override Constant RightShift(Constant other, Scope scope) {
            if (other is Number) {
                return new Number((int)Value >> (int)((Number)other).Value);
            }

            return base.RightShift(other, scope);
        }

        public override Constant Negation(Scope scope) {
            return new Number(-Value);
        }

        public override Constant PostfixIncrement(Scope scope) {
            return new Number(Value++);
        }

        public override Constant PostfixDecrement(Scope scope) {
            return new Number(Value--);
        }

        public override Constant PrefixIncrement(Scope scope) {
            return new Number(++Value);
        }

        public override Constant PrefixDecrement(Scope scope) {
            return new Number(--Value);
        }

        public override string ToString() {
            return Value.ToString();
        }

        public override Constant TypeOf(Scope scope) {
            return new String("number");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value.ToString());
        }

        public override string ToDebugString() {
            return ToString();
        }
    }

    public class Boolean : Constant {
        public bool Value;

        public Boolean(bool value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Boolean", scope).Get(key.ToString());
        }

        public override Constant Add(Constant other, Scope scope) {
            if(other is String) {
                return new String((Value ? Tokens.True : Tokens.False) + ((String)other).Value);
            }

            return base.Add(other, scope);
        }

        public override Constant Equals(Constant other, Scope scope) {
            if(other is Boolean) {
                return new Boolean(Value == ((Boolean)other).Value);
            } else if (other is Number) {
                return new Boolean((Value ? 1 : 0) == ((Number)other).Value);
            } else if (other is String) {
                return new Boolean((Value ? "1" : "0") == ((String)other).Value);
            }

            return new Boolean(false);
        }

        public override Constant StrictEquals(Constant other, Scope scope) {
            if (other is Boolean) {
                return new Boolean(Value == ((Boolean)other).Value);
            }

            return new Boolean(false);
        }

        public override Constant LogicalAnd(Constant other, Scope scope) {
            if (other is Boolean) {
                return new Boolean(Value && ((Boolean)other).Value);
            }

            return base.LogicalAnd(other, scope);
        }

        public override Constant LogicalOr(Constant other, Scope scope) {
            if (other is Boolean) {
                return new Boolean(Value || ((Boolean)other).Value);
            }

            return base.LogicalOr(other, scope);
        }

        public override Constant LogicalNot(Scope scope) {
            return new Boolean(!Value);
        }

        public override string ToString() {
            return Value ? Tokens.True : Tokens.False;
        }

        public override Constant TypeOf(Scope scope) {
            return new String("boolean");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value ? Tokens.True : Tokens.False);
        }

        public override string ToDebugString() {
            return ToString();
        }
    }

    public class Array : Constant {
        public List<Constant> List { get; }

        public Array(int length = 0) {
            List = new List<Constant>();

            for(var i = 0; i < length; i++) {
                List.Add(Static.Undefined);
            }
        }

        public Constant Get(int index) {
            if (index >= 0 && index < List.Count) {
                return List[index];
            } else {
                return Static.Undefined;
            }
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            if (key is Number) {
                return Get((int)((Number)key).Value);
            }

            var keyString = key.ToString();

            int index;
            if(int.TryParse(keyString, out index)) {
                return Get(index);
            }

            if(keyString == "length") {
                return new Number(List.Count);
            }

            return Tool.Construct("Array", scope).Get(keyString);
        }

        public override void SetProperty(Constant key, Constant value, Scope scope) {
            if (key is Number) {
                var index = (int)((Number)key).Value;
                if (index >= 0 && index < List.Count) {
                    List[index] = value;
                }
            }
        }

        public override string ToString() {
            return "[ x" + List.Count + " ]";
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Object.UnevalArray(List.Select(item => (Expression)item).ToList(), builder, depth);
        }

        public override string ToDebugString() {
            return $"[{string.Join(", ", List.Select(item => item.ToDebugString()))}]";
        }
    }


    public class Date : Constant {
        public DateTime Value { get; set; }

        public Date(DateTime value) {
            Value = value;
        }

        public override Constant GetProperty(Constant key, Scope scope) {
            return Tool.Construct("Date", scope).Get(key.ToString());
        }

        public override string ToString() {
            return Value.ToString();
        }

        public override Constant TypeOf(Scope scope) {
            return new String("object");
        }

        public override void Uneval(StringBuilder builder, int depth) {

        }

        public override string ToDebugString() {
            return $"Date({Value.ToString()})";
        }
    }

    public class Variable : Constant {
        public string Name { get; }

        public Variable(string name) {
            Name = name;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            if (getValue) {
                return GetValue(scope);
            } else {
                return this;
            }
        }

        public override Constant GetValue(Scope scope) {
            return scope.GetVariable(Name);
        }

        public override Constant Assignment(Constant other, Scope scope) {
            scope.SetVariable(Name, other);

            // For performance, so not everything is output as a string
            return Static.Undefined;

            //return other;
        }

        public override string ToString() {
            return Name;
        }

        public override string ToDebugString() {
            return Name;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Name);
        }
    }

    public class TypedVariable : Variable {
        public string Type { get; }

        public TypedVariable(string name, string type) : base(name) {
            Type = type;
        }

        public override Constant Assignment(Constant other, Scope scope) {
            if(!Tool.CheckType(other, Type)) {
                throw new TypeError($"Cannot assign value with type '{other.GetType()}' to static type '{Type}'");
            }

            return base.Assignment(other, scope);
        }

        public override string ToDebugString() {
            return Name + ": " + Type;
        }
    }

    public class Path : Constant {
        public List<Constant> Parts = new List<Constant>();

        public override Constant Execute(Scope scope, bool getValue = true) {
            if (getValue) {
                return GetValue(scope);
            } else {
                return this;
            }
        }

        public override Constant GetValue(Scope scope) {
            return Get(0, scope);
        }

        public Constant GetThis(Scope scope) {
            return Get(1, scope);
        }

        public Constant Get(int offset, Scope scope) {
            Constant current = Parts[0];
            if (current is Variable) {
                current = scope.GetVariable(current.ToString());
            }

            for (var i = 1; i < Parts.Count - offset; i++) {
                current = current.GetProperty(Parts[i], scope);
            }

            return current;
        }

        public override Constant Access(Constant other, Scope scope) {
            Parts.Add(other);
            return this;
        }

        public override Constant Assignment(Constant other, Scope scope) {
            var _this = GetThis(scope);

            var property = Parts[Parts.Count - 1];
            _this.SetProperty(property, other, scope);

            // For performance, so not everything is output as a string
            return Static.Undefined;

            //return other;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            for(var i = 0; i < Parts.Count; i++) {
                if (i > 0) builder.Append(".");
                Parts[i].Uneval(builder, depth);
            }
        }

        public override string ToDebugString() {
            return string.Join(".", Parts.Select(part => part.ToDebugString()));
        }
    }

    public abstract class Constant : Expression {

        public override Constant Execute(Scope scope, bool getValue = true) {
            return this;
        }

        public override Constant GetValue(Scope scope) {
            return this;
        }

        public string GetString(Scope scope) {
            var constant = GetValue(scope);

            if (constant is String) {
                return ((String)constant).Value;
            } else if (constant is Number) {
                return ((Number)constant).Value.ToString();
            }

            return "";
        }

        public virtual T As<T>() where T : Constant {
            return (T)this;
        }

        public abstract string ToDebugString();

        public virtual Constant GetProperty(Constant key, Scope scope) {
            throw new ReferenceError($"Cannot access property '{key}' of {ToDebugString()}");
        }

        public virtual void SetProperty(Constant key, Constant value, Scope scope) {
            throw new ReferenceError($"Cannot assign property '{key}' of {ToDebugString()}");
        }

        private Exception OperatorException(string name, Constant a, Constant b) {
            throw new SyntaxError("Can't " + name + " " + a.ToDebugString() + " with " + b.ToDebugString());
        }

        private Exception OperatorException(string name, Constant a) {
            throw new SyntaxError("Can't " + name + " with " + a.ToDebugString());
        }

        public virtual Constant Add(Constant other, Scope scope) {
            throw OperatorException("add", this, other);
        }

        public virtual Constant Substract(Constant other, Scope scope) {
            throw OperatorException("substract", this, other);
        }

        public virtual Constant Multiply(Constant other, Scope scope) {
            throw OperatorException("multiply", this, other);
        }

        public virtual Constant Divide(Constant other, Scope scope) {
            throw OperatorException("divide", this, other);
        }

        public virtual Constant Call(Constant other, Constant _this, Scope scope) {
            throw OperatorException("call", this, other);
        }

        public virtual Constant Access(Constant other, Scope scope) {
            var path = new Path();
            path.Parts.Add(this);

            if (other is Variable || other is Number || other is String) {
                path.Parts.Add(other);
            } else {
                return Static.Undefined;
            }

            return path;
        }

        public virtual Constant Assignment(Constant other, Scope scope) {
            throw OperatorException("assign", this, other);
        }

        public virtual Constant New(Scope scope) {
            throw OperatorException("new", this);
        }

        public virtual Constant LogicalAnd(Constant other, Scope scope) {
            throw OperatorException("logical and", this, other);
        }

        public virtual Constant LogicalOr(Constant other, Scope scope) {
            throw OperatorException("logical or", this, other);
        }

        public virtual Constant LogicalNot(Scope scope) {
            return new Boolean(!IsTrue(scope));
        }

        public virtual Constant TypeOf(Scope scope) {
            return new String("undefined");
        }

        public virtual Constant Void(Scope scope) {
            return Static.Undefined;
        }

        public virtual Constant Delete(Scope scope) {
            // TODO: delete properties of objects
            return new Boolean(false);
        }

        public virtual Constant LessThan(Constant other, Scope scope) {
            throw OperatorException("less than", this, other);
        }

        public virtual Constant LessThanEquals(Constant other, Scope scope) {
            throw OperatorException("less than or equals", this, other);
        }

        public virtual Constant GreaterThan(Constant other, Scope scope) {
            throw OperatorException("greater than", this, other);
        }

        public virtual Constant GreaterThanEquals(Constant other, Scope scope) {
            throw OperatorException("greater than or equals", this, other);
        }

        public virtual Constant In(Constant other, Scope scope) {
            throw OperatorException("in", this, other);
        }

        public virtual Constant Conditional(Constant other, Scope scope) {
            throw OperatorException("conditional", this, other);
        }

        public virtual Constant BitwiseNot(Scope scope) {
            throw OperatorException("bitwise not", this);
        }

        public virtual Constant Negation(Scope scope) {
            throw OperatorException("negation", this);
        }

        public virtual Constant Remainder(Constant other, Scope scope) {
            throw OperatorException("remainder", this, other);
        }

        public virtual Constant LeftShift(Constant other, Scope scope) {
            throw OperatorException("left shift", this, other);
        }

        public virtual Constant RightShift(Constant other, Scope scope) {
            throw OperatorException("right shift", this, other);
        }

        public virtual Constant Equals(Constant other, Scope scope) {
            return new Boolean(false);
        }

        public virtual Constant NotEquals(Constant other, Scope scope) {
            return new Boolean(!((Boolean)Equals(other, scope)).Value);
        }

        public virtual Constant StrictEquals(Constant other, Scope scope) {
            return new Boolean(false);
        }

        public virtual Constant StrictNotEquals(Constant other, Scope scope) {
            return new Boolean(!((Boolean)StrictEquals(other, scope)).Value);
        }

        public virtual Constant BitwiseAnd(Constant other, Scope scope) {
            throw OperatorException("bitwise and", this, other);
        }

        public virtual Constant BitwiseXor(Constant other, Scope scope) {
            throw OperatorException("bitwise xor", this, other);
        }

        public virtual Constant BitwiseOr(Constant other, Scope scope) {
            throw OperatorException("bitwise or", this, other);
        }

        public virtual Constant Comma(Constant other, Scope scope) {
            throw OperatorException("comma", this, other);
        }

        public virtual Constant PostfixIncrement(Scope scope) {
            throw OperatorException("postfix increment", this);
        }

        public virtual Constant PostfixDecrement(Scope scope) {
            throw OperatorException("postfix decrement", this);
        }

        public virtual Constant PrefixIncrement(Scope scope) {
            throw OperatorException("prefix increment", this);
        }

        public virtual Constant PrefixDecrement(Scope scope) {
            throw OperatorException("prefix decrement", this);
        }
    }
}
