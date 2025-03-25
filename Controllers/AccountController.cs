using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class AccountController : Controller
    {
        AccountModel AccountObj = new();
        public IActionResult Login()
        {
            return View();
        }

        // Loads the Register View
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(AccountModel user)
        {
            bool res;
            if (ModelState.IsValid)
            {
                AccountObj = new AccountModel();
                res = AccountObj.Insert(user);
                if (res)
                {
                    TempData["msg"] = "Added successfully";
                }
                else
                {
                    TempData["msg"] = "Not Added. something went wrong..!!";
                }
            }
            return View();
        }
        public IActionResult Adminlogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Replace this with actual admin credentials check or DB check
                if (model.Email == "admin@quickcart.com" && model.Password == "admin123")
                {
                    // You can use session or authentication
                    HttpContext.Session.SetString("Admin", model.Email);
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid admin login");
                }
            }
            return View("Adminlogin");
        }



    }
}
