using System;
using System.Text;

namespace NetJS.Core.Javascript {

    public class Access : BinaryOperator {
        // TODO: better name
        public bool IsDot;

        public Access(bool isDot) : base(16) {
            IsDot = isDot;
        }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Access(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope, !IsDot);

            var result = Execute(left, right, scope);
            return getValue ? result.GetValue(scope) : result;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);

            if (IsDot) {
                builder.Append(Tokens.Access);
                Right.Uneval(builder, depth);
            } else {
                builder.Append(Tokens.ArrayOpen);
                Right.Uneval(builder, depth);
                builder.Append(Tokens.ArrayClose);
            }
        }

        public override string ToDebugString() {
            return "access";
        }
    }

    public class Call : BinaryOperator {
        public Call() : base(16) { }

        public override Constant Execute(Scope scope, bool getValue = true) {
            Constant _this = Static.Undefined;

            var left = Left.Execute(scope, false);

            if (left is Path path) {
                _this = path.GetThis(scope);
            }
            
            left = left.Execute(scope);
            if (left is Constructor constructor) {
                _this = new Object(constructor.Function.Get<Object>("prototype"));
            }

            if (Right is ArgumentList arguments) {
                return left.Call(arguments, _this, scope);
            } else {
                // TODO: better error message
                throw new InternalError("Call without arguments... Contact developer, this should not happen");
            }
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "call";
        }
    }

    public class New : UnaryRightOperator {
        public New() : base(16) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.New(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.New + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "new";
        }
    }

    public class PostfixIncrement : UnaryLeftOperator {
        public PostfixIncrement() : base(15) { }

        public override Constant Execute(Constant left, Scope scope) {
            return left.PostfixIncrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(Tokens.Increment);
        }

        public override string ToDebugString() {
            return "postfix increment";
        }
    }

    public class PostfixDecrement : UnaryLeftOperator {
        public PostfixDecrement() : base(15) { }

        public override Constant Execute(Constant left, Scope scope) {
            return left.PostfixDecrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(Tokens.Decrement);
        }

        public override string ToDebugString() {
            return "postfix decrement";
        }
    }

    public class LogicalNot : UnaryRightOperator {
        public LogicalNot() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.LogicalNot(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.LogicalNot);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical not";
        }
    }

    public class BitwiseNot : UnaryRightOperator {
        public BitwiseNot() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.BitwiseNot(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.BitwiseNot);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "bitwise not";
        }
    }

    public class Negation : UnaryRightOperator {
        public Negation() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.Negation(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Substract);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "negation";
        }
    }

    public class PrefixIncrement : UnaryRightOperator {
        public PrefixIncrement() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.PrefixIncrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Increment);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "prefix increment";
        }
    }

    public class PrefixDecrement : UnaryRightOperator {
        public PrefixDecrement() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.PrefixDecrement(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Decrement);
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "prefix decrement";
        }
    }

    public class TypeOf : UnaryRightOperator {
        public TypeOf() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.TypeOf(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.TypeOf + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "typeof";
        }
    }

    public class Void : UnaryRightOperator {
        public Void() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.Void(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Void + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "void";
        }
    }

    public class Delete : UnaryRightOperator {
        public Delete() : base(14) { }

        public override Constant Execute(Constant right, Scope scope) {
            return right.Delete(scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Tokens.Delete + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "delete";
        }
    }

    public class Multiplication : BinaryOperator {
        public Multiplication() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Multiply(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Multiply + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "multiplication";
        }
    }

    public class Division : BinaryOperator {
        public Division() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Divide(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Divide + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "division";
        }
    }

    public class Remainder : BinaryOperator {
        public Remainder() : base(13) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Remainder(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Remainder + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "remainder";
        }
    }

    public class Addition : BinaryOperator {
        public Addition() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Add(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Add + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "addition";
        }
    }

    public class Substraction : BinaryOperator {
        public Substraction() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Substract(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Substract + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "substraction";
        }
    }

    public class LeftShift : BinaryOperator {
        public LeftShift() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LeftShift(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LeftShift + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "left shift";
        }
    }

    public class RightShift : BinaryOperator {
        public RightShift() : base(12) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.RightShift(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.RightShift + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "right shift";
        }
    }

    public class InstanceOf : BinaryOperator {
        public InstanceOf() : base(11) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.InstanceOf(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.InstanceOf + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "instanceof";
        }
    }

    public class LessThan : BinaryOperator {
        public LessThan() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LessThan(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LessThan + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "less than";
        }
    }

    public class LessThanEquals : BinaryOperator {
        public LessThanEquals() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LessThanEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LessThanEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "less than equals";
        }
    }

    public class GreaterThan : BinaryOperator {
        public GreaterThan() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.GreaterThan(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.GreaterThan + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "greater than";
        }
    }

    public class GreaterThanEquals : BinaryOperator {
        public GreaterThanEquals() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.GreaterThanEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.GreaterThanEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "greater than equals";
        }
    }

    public class In : BinaryOperator {
        public In() : base(10) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.In(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.In + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "in";
        }
    }

    public class Equals : BinaryOperator {
        public Equals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Equals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Equals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "equals";
        }
    }

    public class NotEquals : BinaryOperator {
        public NotEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.NotEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.NotEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "not equals";
        }
    }

    public class StrictEquals : BinaryOperator {
        public StrictEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.StrictEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.StrictEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "strict equals";
        }
    }

    public class StrictNotEquals : BinaryOperator {
        public StrictNotEquals() : base(9) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.StrictNotEquals(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.StrictNotEquals + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "strict not equals";
        }
    }

    public class BitwiseAnd : BinaryOperator {
        public BitwiseAnd() : base(8) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseAnd(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.BitwiseAnd + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "bitwise and";
        }
    }

    public class BitwiseXor : BinaryOperator {
        public BitwiseXor() : base(7) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseXor(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.BitwiseXor + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "bitwise xor";
        }
    }

    public class BitwiseOr : BinaryOperator {
        public BitwiseOr() : base(6) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.BitwiseOr(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.BitwiseOr + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "bitwise or";
        }
    }

    public class LogicalAnd : BinaryOperator {
        public LogicalAnd() : base(5) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LogicalAnd(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LogicalAnd + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical and";
        }
    }

    public class LogicalOr : BinaryOperator {
        public LogicalOr() : base(4) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.LogicalOr(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.LogicalOr + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "logical or";
        }
    }

    public class Conditional : BinaryOperator {
        public Conditional() : base(3) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Conditional(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var right = Right.Execute(scope);
            Constant result = null;

            if (right is ArgumentList) {
                var list = (ArgumentList)right;
                if (list.Arguments.Count == 2) {
                    if (Left.IsTrue(scope)) {
                        result = list.Arguments[0].Execute(scope);
                    } else {
                        result = list.Arguments[1].Execute(scope);
                    }
                }
            }

            if(result == null) {
                result = Execute(Left.Execute(scope), right, scope);
            }

            return result;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            if (Right is ArgumentList) {
                var list = (ArgumentList)Right;

                Left.Uneval(builder, depth);
                builder.Append(" " + Tokens.Conditional + " ");
                list.Arguments[0].Uneval(builder, depth);
                builder.Append(" " + Tokens.ConditionalSeperator + " ");
                list.Arguments[1].Uneval(builder, depth);
            }
        }

        public override string ToDebugString() {
            return "conditional";
        }
    }

    public class Assignment : BinaryOperator {
        public Assignment() : base(2) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Assignment(right, scope);
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            // TODO: error if left not variable or access
            var left = Left.Execute(scope, false);
            var right = Right.Execute(scope);
            return Execute(left, right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(" " + Tokens.Assign + " ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "assigment";
        }
    }

    public class Comma : BinaryOperator {
        public Comma() : base(1) { }

        public override Constant Execute(Constant left, Constant right, Scope scope) {
            return left.Comma(right, scope);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Left.Uneval(builder, depth);
            builder.Append(", ");
            Right.Uneval(builder, depth);
        }

        public override string ToDebugString() {
            return "comma";
        }
    }

    public abstract class UnaryLeftOperator : Operator {
        public Expression Left;

        public UnaryLeftOperator(int precedence) : base(precedence) { }

        public override bool AcceptsLeft => true;
        public override bool HasLeft => Left != null;
        public override Expression GetLeft => Left;

        public override void SetLeft(Expression left) {
            Left = left;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope);
            return Execute(left, scope);
        }

        public virtual Constant Execute(Constant left, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }

    public abstract class UnaryRightOperator : Operator {
        public Expression Right;

        public UnaryRightOperator(int precedence) : base(precedence) { }

        public override bool AcceptsRight => true;
        public override bool HasRight => Right != null;
        public override Expression GetRight => Right;

        public override void SetRight(Expression right) {
            Right = right;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var right = Right.Execute(scope);
            return Execute(right, scope);
        }

        public virtual Constant Execute(Constant right, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }

    public abstract class BinaryOperator : Operator {
        public Expression Left;
        public Expression Right;

        public BinaryOperator(int precedence) : base(precedence) { }

        public override bool AcceptsLeft => true;
        public override bool AcceptsRight => true;

        public override bool HasLeft => Left != null;
        public override bool HasRight => Right != null;

        public override Expression GetLeft => Left;
        public override Expression GetRight => Right;

        public override void SetLeft(Expression left) {
            Left = left;
        }

        public override void SetRight(Expression right) {
            Right = right;
        }

        public override Constant Execute(Scope scope, bool getValue = true) {
            var left = Left.Execute(scope);
            var right = Right.Execute(scope);
            return Execute(left, right, scope);
        }

        public virtual Constant Execute(Constant left, Constant right, Scope scope) {
            throw new Exception("This operator cannot be called");
        }
    }

    public abstract class Operator : Expression {
        public Operator Parent;
        public float Precedence;

        public Operator(int precedence) {
            Precedence = precedence;
        }

        public virtual bool AcceptsLeft => false;
        public virtual bool AcceptsRight => false;

        public virtual bool HasLeft => false;
        public virtual bool HasRight => false;

        public virtual void SetLeft(Expression left) { }
        public virtual void SetRight(Expression right) { }

        public virtual Expression GetLeft => null;
        public virtual Expression GetRight => null;
    }
}
