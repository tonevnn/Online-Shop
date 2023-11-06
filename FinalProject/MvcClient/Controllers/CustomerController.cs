using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MvcClient.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MvcClient.Controllers
{
    public class CustomerController : Controller
    {
        public string baseUrl = "http://localhost:5024/api/";
        public async Task<IActionResult> Info()
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Customer/GetCustomer");

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    CustomerInfoView cus = JsonConvert.DeserializeObject<CustomerInfoView>(results);
                    ViewData["customerInfo"] = cus;
                    HttpContext.Session.SetString("cusName", cus.ContactName!);
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

        public async Task<IActionResult> Order()
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Order/GetCustomerOrder");

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    List<OrderView> orders = JsonConvert.DeserializeObject<List<OrderView>>(results);
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
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
            }
            return View();
        }

        public async Task<IActionResult> OrderCanceled()
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(baseUrl);
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string token = HttpContext.Session.GetString("token");
                Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await Client.GetAsync("Order/GetCustomerCanceledOrder");

                if (response.IsSuccessStatusCode)
                {
                    string results = response.Content.ReadAsStringAsync().Result;
                    List<OrderView> orders = JsonConvert.DeserializeObject<List<OrderView>>(results);
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
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
            }
            return View("~/Views/Customer/Order.cshtml");
        }
    }
}
