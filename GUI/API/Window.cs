using System;
using System.Drawing;
using System.Windows.Forms;

namespace NetJS.GUI.API {
    public class Window {

        private static void HandleMouseEvent(MouseEventArgs e, dynamic f) {
            if (f != null) {
                f(e.X, e.Y);
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

        private static void HandleKeyEvent(KeyEventArgs e, dynamic f) {
            if (f != null) {
                f(e.KeyValue, GetChar(e).ToString());
            }
        }

        public Form Form;

        public dynamic onmousemove;
        public dynamic onmouseclick;
        public dynamic onmouseup;
        public dynamic onmousedown;
        public dynamic onkeyup;
        public dynamic onkeydown;
        public dynamic onclose;

        public int width;
        public int height;

        public Window(dynamic options) {
            width = !Tool.IsUndefined(options.width) ? options.width : 800;
            height = !Tool.IsUndefined(options.height) ? options.height : 600;

            Form = new Form();
            Form.ClientSize = new Size(width, height);
            Form.Name = "";
            Form.Visible = true;

            Form.MouseMove += (sender, e) => HandleMouseEvent(e, onmousemove);
            Form.MouseClick += (sender, e) => HandleMouseEvent(e, onmouseclick);
            Form.MouseUp += (sender, e) => HandleMouseEvent(e, onmouseup);
            Form.MouseDown += (sender, e) => HandleMouseEvent(e, onmousedown);
            
            Form.KeyUp += (sender, e) => HandleKeyEvent(e, onkeyup);
            Form.KeyDown += (sender, e) => HandleKeyEvent(e, onkeydown);

            Form.FormClosed += (sender, e) => {
                if (onclose != null) {
                    onclose();
                }
            };
        }
        
        public static void doEvents() {
            Application.DoEvents();
        }
        
        public static void exit() {
            Environment.Exit(0);
        }
    }
}
