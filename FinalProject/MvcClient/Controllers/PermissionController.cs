using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using System.Net.Http.Headers;
using MvcClient.Utils;
namespace MvcClient.Controllers
{
    public class PermissionController : Controller
    {
        public IActionResult Login(string error)
        {
            HttpContext.Session.Remove("isAdmin");
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("cusName");
            ViewData["error"] = error;
            return View("~/Views/Login.cshtml");
        }

        public async Task<ActionResult> LoginAccount(LoginView loginView)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Login.cshtml");
            }
            else
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("http://localhost:5024/api/");
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string token = HttpContext.Session.GetString("token");
                    HttpResponseMessage response = await Client.PostAsJsonAsync("Login", loginView);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        HttpContext.Session.SetString("token", result.Substring(1, result.Length - 2));
                        return RedirectToAction("Product", "Admin");

                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission", new { error = "Email or Password is not correct!" });
                    }
                }
            }
        }

        public IActionResult Register()
        {
            return View("~/Views/Register.cshtml");
        }

        public async Task<ActionResult> RegistAccount(RegisterView registerView)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Register.cshtml");
            }
            else
            {

                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("http://localhost:5024/api/");
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await Client.PostAsJsonAsync("Register", registerView);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Permission");
                    }
                }
            }
        }
        public IActionResult ForgotPass()
        {
            return View("~/Views/ForgotPass.cshtml");
        }

        public async Task<IActionResult> ResetPassword(EmailView view)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri("http://localhost:5024/api/");
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await Client.PostAsJsonAsync("ForgotPassword?email=" + view.Email, "");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Permission");
                }
                else
                {
                    return RedirectToAction("Login", "Permission");
                }
            }
        }
    }
}
