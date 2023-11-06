using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public string baseUrl = "http://localhost:5024/api/";

        public async Task<IActionResult> Index()
        {
            using (var Client = new HttpClient())
            {
                var categories = await Common.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;

                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage productHotResponse = await Client.GetAsync("Product/GetProductHot");
                if (productHotResponse.IsSuccessStatusCode)
                {
                    string results = productHotResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> productsHot = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["productsHot"] = productsHot;
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
                HttpResponseMessage productsBestSaleResponse = await Client.GetAsync("Product/GetProductBestSale");
                if (productsBestSaleResponse.IsSuccessStatusCode)
                {
                    string results = productsBestSaleResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> productsBestSale = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["productsBestSale"] = productsBestSale;
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }

                HttpResponseMessage productNewResponse = await Client.GetAsync("Product/GetProductNew");
                if (productHotResponse.IsSuccessStatusCode)
                {
                    string results = productNewResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> productsNew = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["productsNew"] = productsNew;
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
                return View();
            }
        }

        public async Task<IActionResult> Filter(int id)
        {
            using (var Client = new HttpClient())
            {
                var categories = await Common.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                var getcategory = await Common.GetCategoryById(id);
                ViewData["getcategory"] = getcategory;
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage productHotResponse = await Client.GetAsync("Product/GetProductByCategory/"+id);
                if (productHotResponse.IsSuccessStatusCode)
                {
                    string results = productHotResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> productsHot = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["productsHot"] = productsHot;
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
                return View();
            }           
        }

        public async Task<IActionResult> Detail(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage productHotResponse = await Client.GetAsync("Product/GetById/" + id);
                if (productHotResponse.IsSuccessStatusCode)
                {
                    string results = productHotResponse.Content.ReadAsStringAsync().Result;
                    ProductView productsHot = JsonConvert.DeserializeObject<ProductView>(results);
                    ViewData["productsHot"] = productsHot;
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
                return View();
            }
        }
        public async Task<ProductEdit> GetProductById(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage productResponse = await Client.GetAsync("Product/GetById/" + id);
                if (productResponse.IsSuccessStatusCode)
                {
                    string results = productResponse.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<ProductEdit>(results);
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
