using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class AdminController : Controller
    {
        AdminModel AdminObj = new();
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ValidateAdmin(model.UserName, model.Password))
                {
                    HttpContext.Session.SetString("AdminUser", model.UserName);
                    TempData["LoginSuccess"] = "You have logged in successfully!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password!"); // ✅ Fix: Properly store error message
                }
            }
            return View(model); // ✅ Ensure model is passed back to the view
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
            HttpContext.Session.Clear(); // ✅ Ensure session is fully cleared
            TempData["LogoutSuccess"] = "You have logged out successfully!";
            return RedirectToAction("AdminLogin"); // ✅ Redirect to login page
        }

        public IActionResult AdminProfile()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
            {
                return RedirectToAction("AdminLogin");
            }

            // Create an AdminModel object and set the username from the session
            AdminModel admin = new AdminModel
            {
                UserName = HttpContext.Session.GetString("AdminUser")
            };

            return View(admin);
        }
        public IActionResult AllProducts()
        {
            return View();
        }
        
        public IActionResult AddProduct()
        {
            return View();
        }
          



    }
}
