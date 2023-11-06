using MvcClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ClientApp.Utils
{
    public class Calculate
    {
        public static string baseUrl = "http://localhost:5024/api/";
        public static async Task<HttpResponseMessage> callGetApi(string url )
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.GetAsync(url);
                return response;
            }
        }
    }
}
