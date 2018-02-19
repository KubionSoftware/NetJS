using System;

namespace NetJS.External {
    public class IO {

        public static Javascript.Constant write(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.write");
            var content = Tool.GetArgument<Javascript.String>(arguments, 1, "IO.write");

            // TODO: handle errors
            System.IO.File.WriteAllText(scope.Application.Settings.Root + name.Value, content.Value);

            return Javascript.Static.Undefined;
        }

        public static Javascript.Constant read(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.read");

            // TODO: determine return when error
            try {
                return new Javascript.String(System.IO.File.ReadAllText(scope.Application.Settings.Root + name.Value));
            }catch(Exception) {
                return Javascript.Static.Undefined;
            }
        }

        public static Javascript.Constant delete(Javascript.Constant _this, Javascript.Constant[] arguments, Javascript.Scope scope) {
            var name = Tool.GetArgument<Javascript.String>(arguments, 0, "IO.delete");

            // TODO: handle errors
            System.IO.File.Delete(scope.Application.Settings.Root + name.Value);
            return Javascript.Static.Undefined;
        }
    }
}