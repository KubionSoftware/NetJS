using Microsoft.ClearScript.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetJS.API {
    public class Base64 {

        /// <summary>Decodes a base64 string.</summary>
        /// <param name="base64">The string to decode</param>
        /// <returns>Returns the decoded string.</returns>
        /// <example><code lang="javascript">var decoded = Base64.decode("TWFuIGlzIGRpc3Rpbmd1aXNoZWQ=");</code></example>
        public static string decode(string base64) {
            var base64EncodedBytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>Encodes a string as a base64 string.</summary>
        /// <param name="value">The string to encode</param>
        /// <returns>Returns the encoded string.</returns>
        /// <example><code lang="javascript">var encoded = Base64.encode("Man is distinguished");</code></example>
        public static string encode(string value) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(plainTextBytes);
        }
        
        /// <summary>Encodes a byte array as a base64 string.</summary>
        /// <param name="bytes">The bytes to encode</param>
        /// <returns>Returns the encoded string.</returns>
        /// <example><code lang="javascript">var bytes = IO.readBytes("image.png");
        /// var encoded = Base64.encodeBytes(bytes);</code></example>
        public static string encodeBytes(dynamic bytes) {
            var content = (IArrayBuffer)bytes.buffer;
            return Convert.ToBase64String(content.GetBytes());
        }
    }
}