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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            var user = AccountObj.Login(Email, Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id); // <--- This line is missing
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetString("UserLastName", user.LastName);
                HttpContext.Session.SetString("UserPhone", user.PhoneNo);
                HttpContext.Session.SetString("UserAddress", user.Address);

                TempData["LoginSuccess"] = "You have successfully logged in!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AccountModel user)
        {
            if (ModelState.IsValid)
            {
                bool res = AccountObj.Insert(user);
                if (res)
                {
                    TempData["msg"] = "Registration successful! Please login with your credentials.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["msg"] = "Email already exists. Please try again.";
                }
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "Logout Successfully";
            return RedirectToAction("Index", "Home");
        }
    }
}
