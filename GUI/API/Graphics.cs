using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetJS.GUI.API {
    public class Graphics {

        private System.Drawing.Graphics _graphics;
        private Bitmap _buffer;
        private System.Drawing.Graphics _bufferGraphics;

        public Graphics(Window window) {
            var form = window.Form;
            _graphics = form.CreateGraphics();

            _buffer = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
            _bufferGraphics = System.Drawing.Graphics.FromImage(_buffer);
        }

        private static Brush GetBrush(string hex) {
            var color = ColorTranslator.FromHtml(hex);
            var brush = new SolidBrush(color);
            return brush;
        }

        private static Pen GetPen(string hex) {
            var color = ColorTranslator.FromHtml(hex);
            var pen = new Pen(color);
            return pen;
        }

        public void fillRect(double x, double y, double width, double height, string hex) {
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var brush = GetBrush(hex);
            _bufferGraphics.FillRectangle(brush, rect);
        }

        public void drawRect(double x, double y, double width, double height, string hex) {
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var pen = GetPen(hex);
            _bufferGraphics.DrawRectangle(pen, rect);
        }

        public void fillEllipse(double x, double y, double width, double height, string hex) {
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            var brush = GetBrush(hex);
            _bufferGraphics.FillEllipse(brush, rect);
        }

        public void drawEllipse(double x, double y, double width, double height, string hex) {
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height); ;
            var pen = GetPen(hex);
            _bufferGraphics.DrawEllipse(pen, rect);
        }

        public void drawLine(double xFrom, double yFrom, double xTo, double yTo, string hex) {
            var from = new Point((int)xFrom, (int)yFrom);
            var to = new Point((int)xTo, (int)yTo);
            var pen = GetPen(hex);
            _bufferGraphics.DrawLine(pen, from, to);
        }

        public void drawString(double x, double y, string s, string hex) {
            var point = new Point((int)x, (int)y);
            var brush = GetBrush(hex);
            _bufferGraphics.DrawString(s, new Font("Arial", 16f), brush, point);
        }

        public void update() {
            _graphics.DrawImage(_buffer, 0, 0);
        }
    }
}
