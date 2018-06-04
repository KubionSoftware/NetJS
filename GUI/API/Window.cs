using NetJS.Core.API;
using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.GUI.API {
    class Window {

        private static void HandleMouseEvent(MouseEventArgs e, Core.Javascript.Object thisObject, string eventName, LexicalEnvironment lex) {
            if (thisObject.Get(eventName) is Function f) {
                var callbackArguments = new ArgumentList(
                    new Number(e.X),
                    new Number(e.Y)
                );
                f.Call(callbackArguments, Static.Undefined, lex);
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

        private static void HandleKeyEvent(KeyEventArgs e, Core.Javascript.Object thisObject, string eventName, LexicalEnvironment lex) {
            if (thisObject.Get(eventName) is Function f) {
                var callbackArguments = new ArgumentList(
                    new Number(e.KeyValue),
                    new Core.Javascript.String(GetChar(e).ToString())
                );
                f.Call(callbackArguments, Static.Undefined, lex);
            }
        }

        public static Constant constructor(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            var thisObject = (Core.Javascript.Object)_this;

            var options = NetJS.Core.Tool.GetArgument<NetJS.Core.Javascript.Object>(arguments, 0, "Window.create");

            int width = 800, height = 600;
            if (options.Has("width")) width = (int)options.Get<Number>("width").Value;
            if (options.Has("height")) height = (int)options.Get<Number>("height").Value;

            var form = new Form();
            form.ClientSize = new Size(width, height);
            form.Name = "";
            form.Visible = true;

            form.MouseMove += (sender, e) => HandleMouseEvent(e, thisObject, "onmousemove", lex);
            form.MouseClick += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseclick", lex);
            form.MouseUp += (sender, e) => HandleMouseEvent(e, thisObject, "onmouseup", lex);
            form.MouseDown += (sender, e) => HandleMouseEvent(e, thisObject, "onmousedown", lex);
            
            form.KeyUp += (sender, e) => HandleKeyEvent(e, thisObject, "onkeyup", lex);
            form.KeyDown += (sender, e) => HandleKeyEvent(e, thisObject, "onkeydown", lex);

            form.FormClosed += (sender, e) => {
                if (thisObject.Get("onclose") is Function f) {
                    f.Call(new ArgumentList(), Static.Undefined, lex);
                }
            };

            thisObject.Set("form", new Foreign(form));

            thisObject.Set("width", new Number(width));
            thisObject.Set("height", new Number(height));

            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant doEvents(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            Application.DoEvents();
            return Static.Undefined;
        }

        [StaticFunction]
        public static Constant exit(Constant _this, Constant[] arguments, LexicalEnvironment lex) {
            Environment.Exit(0);
            return Static.Undefined;
        }
    }
}
