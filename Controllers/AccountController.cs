using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class AccountController : Controller
    {
        // Loads the Login View
        public IActionResult Login()
        {
            return View();
        }

        // Loads the Register View
        public IActionResult Register()
        {
            return View();
        }

        // Optional - Redirect to Login or Register as default
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
    }
}
