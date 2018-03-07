using System.Security.Cryptography;
using System.Text;

namespace Util {
    public static class Hash {

        private static byte[] CalculateHash(string inputString, HashAlgorithm algorithm) {
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHash(string inputString, HashAlgorithm algorithm) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in CalculateHash(inputString, algorithm)) {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string MD5(string inputString) {
            return GetHash(inputString, System.Security.Cryptography.MD5.Create());
        }

        public static string SHA1(string inputString) {
            return GetHash(inputString, System.Security.Cryptography.SHA1.Create());
        }

        public static string SHA256(string inputString) {
            return GetHash(inputString, System.Security.Cryptography.SHA256.Create());
        }

        public static string SHA512(string inputString) {
            return GetHash(inputString, System.Security.Cryptography.SHA512.Create());
        }
    }
}
