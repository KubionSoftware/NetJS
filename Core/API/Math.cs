using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetJS.Core.API {
    public class MathAPI {

        private static Random _random = new Random();

        public static Constant random(Constant _this, Constant[] arguments, Agent agent) {
            return new Number(_random.NextDouble());
        }

        public static Constant floor(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.floor");
            return new Number(System.Math.Floor(number.Value));
        }

        public static Constant ceil(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.ceil");
            return new Number(System.Math.Ceiling(number.Value));
        }

        public static Constant sin(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.sin");
            return new Number(System.Math.Sin(number.Value));
        }

        public static Constant sinh(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.sinh");
            return new Number(System.Math.Sinh(number.Value));
        }

        public static Constant cos(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.cos");
            return new Number(System.Math.Cos(number.Value));
        }

        public static Constant cosh(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.cosh");
            return new Number(System.Math.Cosh(number.Value));
        }

        public static Constant tan(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.tan");
            return new Number(System.Math.Tan(number.Value));
        }

        public static Constant tanh(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.tanh");
            return new Number(System.Math.Tanh(number.Value));
        }

        public static Constant asin(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.asin");
            return new Number(System.Math.Asin(number.Value));
        }

        public static Constant acos(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.acos");
            return new Number(System.Math.Acos(number.Value));
        }

        public static Constant atan(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.atan");
            return new Number(System.Math.Atan(number.Value));
        }

        public static Constant atan2(Constant _this, Constant[] arguments, Agent agent) {
            var numberY = Tool.GetArgument<Number>(arguments, 0, "Math.atan2");
            var numberX = Tool.GetArgument<Number>(arguments, 1, "Math.atan2");
            return new Number(System.Math.Atan2(numberY.Value, numberX.Value));
        }

        public static Constant exp(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.exp");
            return new Number(System.Math.Exp(number.Value));
        }

        public static Constant abs(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.abs");
            return new Number(System.Math.Abs(number.Value));
        }

        public static Constant sqrt(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.sqrt");
            return new Number(System.Math.Sqrt(number.Value));
        }

        public static Constant pow(Constant _this, Constant[] arguments, Agent agent) {
            var baseNumber = Tool.GetArgument<Number>(arguments, 0, "Math.pow");
            var exponentNumber = Tool.GetArgument<Number>(arguments, 1, "Math.pow");
            return new Number(System.Math.Pow(baseNumber.Value, exponentNumber.Value));
        }

        public static Constant round(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.round");
            return new Number(System.Math.Round(number.Value));
        }

        public static Constant log(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.log");
            return new Number(System.Math.Log(number.Value));
        }

        public static Constant log10(Constant _this, Constant[] arguments, Agent agent) {
            var number = Tool.GetArgument<Number>(arguments, 0, "Math.log10");
            return new Number(System.Math.Log10(number.Value));
        }

        public static Constant max(Constant _this, Constant[] arguments, Agent agent) {
            var numbers = new List<double>();
            for(var i = 0; i < arguments.Length; i++) {
                numbers.Add(Tool.GetArgument<Number>(arguments, i, "Math.max").Value);
            }
            return new Number(numbers.Max());
        }

        public static Constant min(Constant _this, Constant[] arguments, Agent agent) {
            var numbers = new List<double>();
            for (var i = 0; i < arguments.Length; i++) {
                numbers.Add(Tool.GetArgument<Number>(arguments, i, "Math.min").Value);
            }
            return new Number(numbers.Min());
        }
    }
}