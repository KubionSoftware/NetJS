using System;
using System.Collections.Generic;
using System.Text;

namespace NetJS.Core.Javascript {
    public class StringBlueprint : Blueprint {
        public string Value { get; private set; }

        public StringBlueprint(string value) {
            Value = value;
        }

        public void Combine(StringBlueprint other) {
            Value += other.Value;
        }

        public override Constant Instantiate(Scope scope) {
            return new String(Value);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append("\"" + Value + "\"");
        }

        public override string ToDebugString() {
            return "stringblueprint";
        }
    }

    public class NumberBlueprint : Blueprint {
        public double Value { get; }

        public NumberBlueprint(double value) {
            Value = value;
        }

        public override Constant Instantiate(Scope scope) {
            return new Number(Value);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value.ToString());
        }

        public override string ToDebugString() {
            return "numberblueprint";
        }
    }

    public class BooleanBlueprint : Blueprint {
        public bool Value { get; }

        public BooleanBlueprint(bool value) {
            Value = value;
        }

        public override Constant Instantiate(Scope scope) {
            return new Boolean(Value);
        }

        public override void Uneval(StringBuilder builder, int depth) {
            builder.Append(Value ? Tokens.True : Tokens.False);
        }

        public override string ToDebugString() {
            return "booleanblueprint";
        }
    }

    public class ObjectBlueprint : Blueprint {
        public Dictionary<string, Expression> Blueprints { get; }

        public ObjectBlueprint(Dictionary<string, Expression> blueprints) {
            Blueprints = blueprints;
        }

        public override Constant Instantiate(Scope scope) {
            var newObject = Tool.Construct("Object", scope);

            foreach (var key in Blueprints.Keys) {
                newObject.Set(key, Blueprints[key].Execute(scope));
            }

            return newObject;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Object.UnevalDictionary(Blueprints, builder, depth);
        }

        public override string ToDebugString() {
            return "objectblueprint";
        }
    }

    public class ArrayBlueprint : Blueprint {
        public List<Expression> Blueprints { get; }

        public ArrayBlueprint(List<Expression> blueprints) {
            Blueprints = blueprints;
        }

        public override Constant Instantiate(Scope scope) {
            var array = new Array();

            for (var i = 0; i < Blueprints.Count; i++) {
                var blueprint = Blueprints[i];
                array.List.Add(blueprint.Execute(scope));
            }

            return array;
        }

        public override void Uneval(StringBuilder builder, int depth) {
            Object.UnevalArray(Blueprints, builder, depth);
        }

        public override string ToDebugString() {
            return "arrayblueprint";
        }
    }

    public class FunctionBlueprint : Blueprint {
        public string Name { get; set; }
        public string Type { get; }
        public ParameterList Parameters { get; }
        public Block Body { get; set; }

        public FunctionBlueprint(string name, string type, ParameterList parameters, Block body) {
            Name = name;
            Type = type;
            Parameters = parameters;
            Body = body;
        }

        public override Constant Instantiate(Scope scope) {
            return new InternalFunction(scope) { Name = Name, Type = Type, Parameters = Parameters, Body = Body };
        }

        public override void Uneval(StringBuilder builder, int depth) {
            InternalFunction.UnevalFunction(builder, depth, Name, Parameters, Body);
        }

        public override string ToDebugString() {
            return "functionblueprint";
        }
    }

    abstract public class Blueprint : Expression {

        public abstract Constant Instantiate(Scope scope);

        public override Constant Execute(Scope scope, bool getValue = true) {
            return Instantiate(scope);
        }
    }
}