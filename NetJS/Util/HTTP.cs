using System;
using System.Net.Http;
using System.Text;

namespace Util {
    public static class HTTP {
        private static HttpClient Client = new HttpClient();

        // string url, string response, long time
        public static Action<string, string, long> OnGet = null;

        // string url, string request, string response, long time
        public static Action<string, string, string, long> OnPost = null;

        public static string DoGet(HttpClient client, string url) {
            try {
                string result = client.GetStringAsync(url).Result;

                return result;
            } catch {
                return "";
            }
        }

        public static string Get(string url) {
            return DoGet(Client, url);
        }

        public static string DoPost(HttpClient client, string url, string text, string mediaType = "text/plain") {
            try {
                HttpResponseMessage response = client.PostAsync(url, new StringContent(text, Encoding.UTF8, mediaType)).Result;
                string result = response.Content.ReadAsStringAsync().Result;

                return result;
            } catch {
                return "";
            }
        }

        public static string Post(string url, string text, string mediaType = "text/plain") {
            return DoPost(Client, url, text, mediaType);
        }
    }
}
