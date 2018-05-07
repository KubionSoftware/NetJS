using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console.API {
    class Console {

        public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var text = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.String>(arguments, 0, "Console.write");
            System.Console.Write(text);
            return Static.Undefined;
        }

        public static Constant writeLine(Constant _this, Constant[] arguments, Scope scope) {
            var text = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.String>(arguments, 0, "Console.writeLine");
            System.Console.WriteLine(text);
            return Static.Undefined;
        }

        public static Constant readLine(Constant _this, Constant[] arguments, Scope scope) {
            return new NetJS.Core.Javascript.String(System.Console.ReadLine());
        }
    }
}
