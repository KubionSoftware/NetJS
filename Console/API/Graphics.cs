using NetJS.Core.Javascript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.Console.API {
    class Graphics {

        public static Constant constructor(Constant _this, Constant[] arguments, Scope scope) {
            var thisObject = (Core.Javascript.Object)_this;

            var window = Core.Tool.GetArgument<Core.Javascript.Object>(arguments, 0, "Graphics constructor");
            var form = (Form)window.Get<Foreign>("form").Value;
            var graphics = form.CreateGraphics();

            thisObject.Set("graphics", new Foreign(graphics));

            var buffer = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
            thisObject.Set("buffer", new Foreign(buffer));
            thisObject.Set("bufferGraphics", new Foreign(System.Drawing.Graphics.FromImage(buffer)));

            return Static.Undefined;
        }

        public static Constant fillRect(Constant _this, Constant[] arguments, Scope scope) {
            var x = (int)Core.Tool.GetArgument<Number>(arguments, 0, "Graphics.fillRect").Value;
            var y = (int)Core.Tool.GetArgument<Number>(arguments, 1, "Graphics.fillRect").Value;
            var width = (int)Core.Tool.GetArgument<Number>(arguments, 2, "Graphics.fillRect").Value;
            var height = (int)Core.Tool.GetArgument<Number>(arguments, 3, "Graphics.fillRect").Value;

            var hex = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 4, "Graphics.fillRect").Value;
            var color = ColorTranslator.FromHtml(hex);
            var brush = new SolidBrush(color);

            var jsGraphics = ((Core.Javascript.Object)_this).Get<Foreign>("bufferGraphics");
            var graphics = (System.Drawing.Graphics)jsGraphics.Value;
            graphics.FillRectangle(brush, x, y, width, height);
            return Static.Undefined;
        }

        public static Constant drawString(Constant _this, Constant[] arguments, Scope scope) {
            var x = (int)Core.Tool.GetArgument<Number>(arguments, 0, "Graphics.drawString").Value;
            var y = (int)Core.Tool.GetArgument<Number>(arguments, 1, "Graphics.drawString").Value;
            var s = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 2, "Graphics.drawString").Value;

            var hex = Core.Tool.GetArgument<Core.Javascript.String>(arguments, 3, "Graphics.drawString").Value;
            var color = ColorTranslator.FromHtml(hex);
            var brush = new SolidBrush(color);

            var jsGraphics = ((Core.Javascript.Object)_this).Get<Foreign>("bufferGraphics");
            var graphics = (System.Drawing.Graphics)jsGraphics.Value;
            graphics.DrawString(s, new Font("Arial", 16f), brush, x, y);
            return Static.Undefined;
        }

        public static Constant update(Constant _this, Constant[] arguments, Scope scope) {
            var jsGraphics = ((Core.Javascript.Object)_this).Get<Foreign>("graphics");
            var graphics = (System.Drawing.Graphics)jsGraphics.Value;

            var jsBuffer = ((Core.Javascript.Object)_this).Get<Foreign>("buffer");
            var buffer = (Bitmap)jsBuffer.Value;

            graphics.DrawImage(buffer, 0, 0);
            return Static.Undefined;
        }
    }
}
