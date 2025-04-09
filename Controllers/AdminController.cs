using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;

namespace ASP.netcore_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminModel AdminObj = new();

        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                bool isValid = AdminObj.ValidateAdmin(model.UserName, model.Password);

                if (isValid)
                {
                    HttpContext.Session.SetString("AdminUser", model.UserName);
                    TempData["LoginSuccess"] = "You have logged in successfully!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password!");
                }
            }

            return View(model);
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
            {
                return RedirectToAction("AdminLogin");
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutSuccess"] = "You have logged out successfully!";
            return RedirectToAction("AdminLogin");
        }

        public IActionResult AdminProfile()
        {
            string? adminUsername = HttpContext.Session.GetString("AdminUser");
            if (adminUsername == null)
            {
                return RedirectToAction("AdminLogin");
            }

            var admin = new AdminModel
            {
                UserName = adminUsername
            };

            return View(admin);
        }

    


    }
}

