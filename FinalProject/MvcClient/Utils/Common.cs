using MvcClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;

namespace MvcClient.Utils
{
    public class Common
    {
        public static string baseUrl = "http://localhost:5024/api/";

        public static async Task<List<CategoryView>> GetAllCategory()
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage categoryResponse = await Client.GetAsync("category/GetAllCate");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    string results = categoryResponse.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<CategoryView>>(results);
                }
                else
                {
                    Console.WriteLine("Error Calling web API");
                    return null;
                }
             }
        }
        public static async Task<CategoryView> GetCategoryById(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage categoryResponse = await Client.GetAsync("category/GetCategoryById/" + id);

                if (categoryResponse.IsSuccessStatusCode)
                {
                    string results = categoryResponse.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<CategoryView>(results);
                }
                else
                {
                    Console.WriteLine("Error Calling web API");
                    return null;
                }
            }
        }
    }
}
