using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace MyApp.ApiTests
{
    public class JsonContent : StringContent
    {
        public JsonContent(object payload) : base(
            JsonConvert.SerializeObject(payload),
            Encoding.UTF8,
            "application/json"
        )
        {
            
        }
    }
}