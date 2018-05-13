using NetJS.Core.API;
using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.Console.API {
    class Window {

        private static void HandleMouseEvent(MouseEventArgs e, Core.Javascript.Object thisObject, string eventName, Scope scope) {
            if (thisObject.Get(eventName) is Function f) {
                var callbackArguments = new ArgumentList(
                    new Number(e.X),
                    new Number(e.Y)
                );
                f.Call(callbackArguments, Static.Undefined, scope);
            }
        }

        private static char GetChar(KeyEventArgs e) {
            int keyValue = e.KeyValue;
            if (!e.Shift && keyValue >= (int)Keys.A && keyValue <= (int)Keys.Z) {
                return (char)(keyValue + 32);
            } else {
                return (char)keyValue;
            }
        }

        private static void HandleKeyEvent(KeyEventArgs e, Core.Javascript.Object thisObject, string eventName, Scope scope) {
            if (thisObject.Get(eventName) is Function f) {
                var callbackArguments = new ArgumentList(
                    new Number(e.KeyValue),
                    new Core.Javascript.String(GetChar(e).ToString())
                );
                f.Call(callbackArguments, Static.Undefined, scope);
            }
        }

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Core.Javascript.Object)_this;

            var options = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.Object>(arguments, 0, "Window.create");
            var form = new Form();
            form.Size = new Size(800, 600);
            form.Name = "";
            form.Visible = true;

            form.MouseMove += (sender, e) => HandleMouseEvent(e, thisObject, "onmousemove", scope);
            form.MouseClick += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseclick", scope);
            form.MouseUp += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseup", scope);
            form.MouseDown += (sender, e) => HandleMouseEvent(e, thisObject, "onmousedown", scope);
            
            form.KeyUp += (sender, e) => HandleKeyEvent(e, thisObject, "onkeyup", scope);
            form.KeyDown += (sender, e) => HandleKeyEvent(e, thisObject, "onkeydown", scope);

            thisObject.Set("form", new Foreign(form));

            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant doEvents(Constant _this, Constant[] arguments, Scope scope) {
            Application.DoEvents();
            return Static.Undefined;
        }
    }
}
