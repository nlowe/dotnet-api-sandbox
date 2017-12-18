using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyApp.ApiTests.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T payload) =>
            await client.PostAsync(
                url,
                new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                )
            );
        
        public static async Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T payload) =>
            await client.PutAsync(
                url,
                new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                )
            );
        
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url) =>
            JsonConvert.DeserializeObject<T>(await client.GetStringAsync(url));
    }
}