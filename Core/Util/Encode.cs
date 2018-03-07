using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util {
    class Encode {
        public static string UrlEncode(string text) {
            return System.Uri.EscapeDataString(text);
        }
        
        public static string UrlDecode(string text) {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            text = text.Replace("+", " ");
            return System.Uri.UnescapeDataString(text);
        }
    }
}
