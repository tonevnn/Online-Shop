using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MvcClient.Models;
using MvcClient.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MvcClient.Controllers
{
    public class AdminController : Controller
    {
        public string baseUrl = "http://localhost:5024/api/";

        public async Task<ActionResult> Product(ProductSearchView? searchView)
        {
            var categories = await Common.GetAllCategory();
            if (categories == null)
            {
                return RedirectToAction("Login", "Permission");
            }
            ViewData["categories"] = categories;

            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage productResponse = await Client.GetAsync("product/GetWithFilter?categoryId=" + searchView.CategoryId + (searchView.Search != null ? ("&search=" + searchView.Search) : ""));

                if (productResponse.IsSuccessStatusCode)
                {
                    string results = productResponse.Content.ReadAsStringAsync().Result;
                    List<ProductView> products = JsonConvert.DeserializeObject<List<ProductView>>(results);
                    ViewData["products"] = products;
                    HttpContext.Session.SetString("isAdmin", "true");
                }
                else if (productResponse.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Error401", "Error401");
                }
                else if (productResponse.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    HttpContext.Session.SetString("isAdmin", "false");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }

            ViewData["token"] = HttpContext.Session.GetString("token");
            return View("~/Views/Admin/Index.cshtml");
        }

        public async Task<ActionResult> Create()
        {
            var categories = await Common.GetAllCategory();
            if (categories == null)
            {
                return RedirectToAction("Login", "Permission");
            }
            ViewData["categories"] = categories;
            return View("~/Views/Admin/Create.cshtml");
        }

        public async Task<ActionResult> Update(int id)
        {
            var categories = await Common.GetAllCategory();
            if (categories == null)
            {
                return RedirectToAction("Login", "Permission");
            }
            ViewData["categories"] = categories;
            return View("~/Views/Admin/Update.cshtml", await GetProductById(id));
        }

        private async Task<ProductEdit> GetProductById(int id)
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

        public async Task<ActionResult> CreateProduct(ProductAdd product)
        {
            if (ModelState.IsValid)
            {
                string token = HttpContext.Session.GetString("token");
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(baseUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    HttpResponseMessage response = await Client.PostAsJsonAsync("Product/Create", product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
            }
            else
            {
                var categories = await Common.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Create.cshtml");
            }
        }



        public async Task<ActionResult> UpdateProduct(ProductEdit product)
        {
            if (ModelState.IsValid)
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(baseUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string token = HttpContext.Session.GetString("token");
                    Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    HttpResponseMessage response = await Client.PutAsJsonAsync("Product/Update", product);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Product));
                    }
                    else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    {
                        return View("~/Views/Home/Index.cshtml");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
            }
            else
            {
                var categories = await Common.GetAllCategory();
                if (categories == null)
                {
                    return RedirectToAction("Login", "Permission");
                }
                ViewData["categories"] = categories;
                return View("~/Views/Admin/Update.cshtml");
            }
        }

        public async Task<ActionResult> DeleteProduct(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.DeleteAsync($"Product/Delete?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Product));
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
        }

        public async Task<ActionResult> Order(OrderSearchView? searchView)
        {
            string param = "";
            if(searchView != null)
            {
                param = "?";
                if (searchView.From == null && searchView.To !=null)
                {
                    param += "&to=" + DateTime.Parse(searchView.To.ToString()).ToString("yyyy-MM-dd");
                }
                if (searchView.To == null && searchView.From != null)
                {
                    param += "&from=" + DateTime.Parse(searchView.From.ToString()).ToString("yyyy-MM-dd");
                }
                if(searchView.From != null && searchView.To != null)
                {
                    param += "&from=" + DateTime.Parse(searchView.From.ToString()).ToString("yyyy-MM-dd");
                    param += "&to=" + DateTime.Parse(searchView.To.ToString()).ToString("yyyy-MM-dd");
                }
            }
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Order/GetAllOrders" + param);

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    List<OrderAdminView> orders = JsonConvert.DeserializeObject<List<OrderAdminView>>(results);
                    ViewData["orders"] = orders;
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            ViewData["searchView"] = searchView;
            return View(searchView);
        }

        public async Task<ActionResult> CancelOrder(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.PutAsJsonAsync("Order/CancelOrder?orderId=" + id, id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Order));
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
        }

        public async Task<ActionResult> OrderDetail(int id)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Order/GetOrderDetail?id=" + id);

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    OrderView orderDetail = JsonConvert.DeserializeObject<OrderView>(results);
                    ViewData["orderDetail"] = orderDetail;
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Forbidden))
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            return View();
        }


        public async Task<IActionResult> Dashboard(int year)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Dashboard/GetDashboard");
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    DashboardView dashboard = JsonConvert.DeserializeObject<DashboardView>(results);
                    ViewData["dashboard"] = dashboard;

                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Dashboard/GetStaticOrder?year=" + year);
                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    List<int> orders = JsonConvert.DeserializeObject<List<int>>(results);
                    ViewData["orders"] = results;

                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
           return View();
        }
    }
}
