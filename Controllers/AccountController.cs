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

        // Optional - Redirect to Login or Register as default
      
    }
}
