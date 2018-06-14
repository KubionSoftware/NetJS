using NetJS.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.GUI.API {
    class Graphics {

        public static Constant constructor(Constant _this, Constant[] arguments, Agent agent) {
            var thisObject = (Core.Object)_this;

            var window = Core.Tool.GetArgument<Core.Object>(arguments, 0, "Graphics constructor");
            var form = (Form)(window.Get("form") as Foreign).Value;
            var graphics = form.CreateGraphics();

            thisObject.Set("graphics", new Foreign(graphics));

            var buffer = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
            thisObject.Set("buffer", new Foreign(buffer));
            thisObject.Set("bufferGraphics", new Foreign(System.Drawing.Graphics.FromImage(buffer)));

            return Static.Undefined;
        }

        private static Brush GetBrush(Constant[] arguments, int index, string context) {
            var hex = Core.Tool.GetArgument<Core.String>(arguments, index, context).Value;
            var color = ColorTranslator.FromHtml(hex);
            var brush = new SolidBrush(color);
            return brush;
        }

        private static Pen GetPen(Constant[] arguments, int index, string context) {
            var hex = Core.Tool.GetArgument<Core.String>(arguments, index, context).Value;
            var color = ColorTranslator.FromHtml(hex);
            var pen = new Pen(color);
            return pen;
        }

        private static Rectangle GetRectangle(Constant[] arguments, string context) {
            var x = (int)Core.Tool.GetArgument<Number>(arguments, 0, context).Value;
            var y = (int)Core.Tool.GetArgument<Number>(arguments, 1, context).Value;
            var width = (int)Core.Tool.GetArgument<Number>(arguments, 2, context).Value;
            var height = (int)Core.Tool.GetArgument<Number>(arguments, 3, context).Value;
            return new Rectangle(x, y, width, height);
        }

        private static Point GetPoint(Constant[] arguments, string context, int start = 0) {
            var x = (int)Core.Tool.GetArgument<Number>(arguments, start, context).Value;
            var y = (int)Core.Tool.GetArgument<Number>(arguments, start + 1, context).Value;
            return new Point(x, y);
        }

        private static System.Drawing.Graphics GetGraphics(Constant _this) {
            var jsGraphics = ((Core.Object)_this).Get("bufferGraphics") as Foreign;
            var graphics = (System.Drawing.Graphics)jsGraphics.Value;
            return graphics;
        }

        public static Constant fillRect(Constant _this, Constant[] arguments, Agent agent) {
            var rect = GetRectangle(arguments, "Graphics.fillRect");
            var brush = GetBrush(arguments, 4, "Graphics.fillRect");
            GetGraphics(_this).FillRectangle(brush, rect);
            return Static.Undefined;
        }

        public static Constant drawRect(Constant _this, Constant[] arguments, Agent agent) {
            var rect = GetRectangle(arguments, "Graphics.drawRect");
            var pen = GetPen(arguments, 4, "Graphics.drawRect");
            GetGraphics(_this).DrawRectangle(pen, rect);
            return Static.Undefined;
        }

        public static Constant fillEllipse(Constant _this, Constant[] arguments, Agent agent) {
            var rect = GetRectangle(arguments, "Graphics.fillEllipse");
            var brush = GetBrush(arguments, 4, "Graphics.fillEllipse");
            GetGraphics(_this).FillEllipse(brush, rect);
            return Static.Undefined;
        }

        public static Constant drawEllipse(Constant _this, Constant[] arguments, Agent agent) {
            var rect = GetRectangle(arguments, "Graphics.drawEllipse");
            var pen = GetPen(arguments, 4, "Graphics.drawEllipse");
            GetGraphics(_this).DrawEllipse(pen, rect);
            return Static.Undefined;
        }

        public static Constant drawLine(Constant _this, Constant[] arguments, Agent agent) {
            var from = GetPoint(arguments, "Graphics.drawLine");
            var to = GetPoint(arguments, "Graphics.drawLine", 2);
            var pen = GetPen(arguments, 4, "Graphics.drawLine");
            GetGraphics(_this).DrawLine(pen, from, to);
            return Static.Undefined;
        }

        public static Constant drawString(Constant _this, Constant[] arguments, Agent agent) {
            var point = GetPoint(arguments, "Graphics.drawString");
            var s = Core.Tool.GetArgument<Core.String>(arguments, 2, "Graphics.drawString").Value;
            var brush = GetBrush(arguments, 3, "Graphics.drawString");
            GetGraphics(_this).DrawString(s, new Font("Arial", 16f), brush, point);
            return Static.Undefined;
        }

        public static Constant update(Constant _this, Constant[] arguments, Agent agent) {
            var jsGraphics = ((Core.Object)_this).Get("graphics") as Foreign;
            var graphics = (System.Drawing.Graphics)jsGraphics.Value;

            var jsBuffer = ((Core.Object)_this).Get("buffer") as Foreign;
            var buffer = (Bitmap)jsBuffer.Value;

            graphics.DrawImage(buffer, 0, 0);
            return Static.Undefined;
        }
    }
}
