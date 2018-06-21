using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Core {
    public class Log {

        public static void Write(string message) {
            var file = "system_log.txt";

            try {
                var info = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss:fff");
                System.IO.File.AppendAllText(file, info + " - " + message + "\n");
            } catch {

            }
        }
    }
}
