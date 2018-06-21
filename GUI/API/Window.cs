using NetJS.Core.API;
using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.GUI.API {
    class Window {

        private static void HandleMouseEvent(MouseEventArgs e, Core.Object thisObject, string eventName, Agent agent) {
            if (thisObject.Get(eventName, agent) is Function f) {
                var callbackArguments = new Constant[] {
                    new Number(e.X),
                    new Number(e.Y)
                };
                f.Call(Static.Undefined, agent, callbackArguments);
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

        private static void HandleKeyEvent(KeyEventArgs e, Core.Object thisObject, string eventName, Agent agent) {
            if (thisObject.Get(eventName, agent) is Function f) {
                var callbackArguments = new Constant[] {
                    new Number(e.KeyValue),
                    new Core.String(GetChar(e).ToString())
                };
                f.Call(Static.Undefined, agent, callbackArguments);
            }
        }

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Core.Object)_this;

            var options = NetJS.Core.Tool.GetArgument<NetJS.Core.Object>(arguments, 0, "Window.create");

            int width = 800, height = 600;
            if (options.HasProperty(new Core.String("width"))) width = (int)(options.Get("width", agent) as Number).Value;
            if (options.HasProperty(new Core.String("height"))) height = (int)(options.Get("height", agent) as Number).Value;

            var form = new Form();
            form.ClientSize = new Size(width, height);
            form.Name = "";
            form.Visible = true;

            form.MouseMove += (sender, e) => HandleMouseEvent(e, thisObject, "onmousemove", agent);
            form.MouseClick += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseclick", agent);
            form.MouseUp += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseup", agent);
            form.MouseDown += (sender, e) => HandleMouseEvent(e, thisObject, "onmousedown", agent);
            
            form.KeyUp += (sender, e) => HandleKeyEvent(e, thisObject, "onkeyup", agent);
            form.KeyDown += (sender, e) => HandleKeyEvent(e, thisObject, "onkeydown", agent);

            form.FormClosed += (sender, e) => {
                if (thisObject.Get("onclose", agent) is Function f) {
                    f.Call(Static.Undefined, agent);
                }
            };

            thisObject.Set("form", new Foreign(form));

            thisObject.Set("width", new Number(width));
            thisObject.Set("height", new Number(height));

            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant doEvents(Constant _this, Constant[] arguments, Agent agent) {
            Application.DoEvents();
            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant exit(Constant _this, Constant[] arguments, Agent agent) {
            Environment.Exit(0);
            return Static.Undefined;
        }
    }
}
