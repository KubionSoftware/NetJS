using System;
using NetJS.Core.Javascript;

namespace NetJS.External {
    public class IO {

        public static Constant write(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.write");
            var content = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 1, "IO.write");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");
            
            // TODO: handle errors
            System.IO.File.WriteAllText(application.Settings.Root + name.Value, content.Value);

            return Static.Undefined;
        }

        public static Constant read(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.read");

            // TODO: determine return when error
            try {
                var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
                if (application == null) throw new InternalError("No application");
                return new Core.Javascript.String(System.IO.File.ReadAllText(application.Settings.Root + name.Value));
            }catch(Exception) {
                return Static.Undefined;
            }
        }

        public static Constant delete(Constant _this, Constant[] arguments, Scope scope) {
            var name = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 0, "IO.delete");

            var application = Tool.GetFromScope<JSApplication>(scope, "__application__");
            if (application == null) throw new InternalError("No application");

            // TODO: handle errors
            System.IO.File.Delete(application.Settings.Root + name.Value);
            return Static.Undefined;
        }
    }
}