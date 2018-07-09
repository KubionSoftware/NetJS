using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetJS.Console.API {
    class Console {

        public static void write(string value) {
            System.Console.Write(value);
        }

        public static void writeLine(string value) {
            System.Console.WriteLine(value);
        }

        public static string readLine() {
            return System.Console.ReadLine();
        }
    }
}
