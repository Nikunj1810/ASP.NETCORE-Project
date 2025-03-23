using System.Diagnostics;
using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger; // Automatically assigned

        public IActionResult Index()
        {
            _logger.LogInformation("Index page accessed.");
            return View();
        }

        public IActionResult Login()
        {
            _logger.LogInformation("Login page accessed.");
            return View();
        }

        public IActionResult Privacy() => View();
        public IActionResult Register() => View();
        public IActionResult AboutUs() => View();
        public IActionResult ContactUs() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}