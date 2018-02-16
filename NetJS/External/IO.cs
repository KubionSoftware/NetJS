using NetJS.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.External {
    public class IO {

        public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.write");
            var content = Tool.GetArgument<Javascript.String>(arguments, 1, "IO.write");

            // TODO: handle errors
            System.IO.File.WriteAllText(scope.Application.Settings.Root + name.Value, content.Value);

            return Static.Undefined;
        }

        public static Constant read(Constant _this, Constant[] arguments, Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.read");

            // TODO: determine return when error
            try {
                return new Javascript.String(System.IO.File.ReadAllText(scope.Application.Settings.Root + name.Value));
            }catch(Exception) {
                return Javascript.Static.Undefined;
            }
        }

        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.delete");

            // TODO: handle errors
            System.IO.File.Delete(scope.Application.Settings.Root + name.Value);
            return Static.Undefined;
        }
    }
}