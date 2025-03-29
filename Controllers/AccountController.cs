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
        public IActionResult Login(string Email, string Password)
        {
            AccountModel user = AccountObj.Login(Email, Password);

            if (user != null)
            {
                // ✅ Store user details in Session
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
                    TempData["msg"] = "Registration failed. Please try again.";
                }
            }
            return View(user);
        }

       
           public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears session data
            TempData["LogoutMessage"] = "Logout Successfully"; // Stores message for the next request
            return RedirectToAction("Index", "Home"); // Redirects to login page
        }

    }
}
