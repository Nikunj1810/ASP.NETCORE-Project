using Microsoft.AspNetCore.Mvc;
using ASP.netcore_Project.Models;

namespace ASP.netcore_Project.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account"); // Redirect if not logged in
            }

            var user = new ProfileModel
            {
                FirstName = HttpContext.Session.GetString("UserFirstName"),
                LastName = HttpContext.Session.GetString("UserLastName"),
                Email = HttpContext.Session.GetString("UserEmail"),
                Phone = HttpContext.Session.GetString("UserPhone"),
                Address = HttpContext.Session.GetString("UserAddress")
            };

            return View(user);
        }
    }
}
