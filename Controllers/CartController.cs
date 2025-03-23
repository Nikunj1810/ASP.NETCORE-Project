using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }
    }
}
