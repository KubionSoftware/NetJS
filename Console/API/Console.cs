using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console.API {
    class Console {

        public static Constant write(Constant _this, Constant[] arguments, Agent agent) {
            var value = Tool.GetArgument(arguments, 0, "Console.write");
            System.Console.Write(Core.Convert.ToString(value, agent));
            return Static.Undefined;
        }

        public static Constant writeLine(Constant _this, Constant[] arguments, Agent agent) {
            var value = Tool.GetArgument(arguments, 0, "Console.writeLine");
            System.Console.WriteLine(Core.Convert.ToString(value, agent));
            return Static.Undefined;
        }

        public static Constant readLine(Constant _this, Constant[] arguments, Agent agent) {
            return new NetJS.Core.String(System.Console.ReadLine());
        }
    }
}
