using Microsoft.AspNetCore.Mvc;

namespace MvcClient.Controllers
{
    public class Error401Controller : Controller
    {
        public IActionResult Error401()
        {
            return View();
        }
    }
}
