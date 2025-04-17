using System.Diagnostics;
using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ASP.netcore_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoCollection<ProductModel> _productCollection;
        private readonly IMongoCollection<ContactUsModel> _contactUsCollection;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
            // Initialize MongoDB connection
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _productCollection = database.GetCollection<ProductModel>("Products");
            _contactUsCollection = database.GetCollection<ContactUsModel>("ContactUs");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index page accessed.");
            
            // Retrieve new arrival products
            var newArrivals = _productCollection.Find(p => p.IsNewArrival == true)
                                               .SortByDescending(p => p.CreatedAt)
                                               .Limit(4)
                                               .ToList();
            
            // Retrieve 10 random products for top selling section
            var topSelling = _productCollection.Aggregate()
                                              .Sample(10)
                                              .ToList();
            
            // Pass data to view using ViewBag
            ViewBag.NewArrivals = newArrivals;
            ViewBag.TopSelling = topSelling;
            
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

        [HttpPost]
        public IActionResult ContactUs(ContactUsModel model)
        {
            if (ModelState.IsValid) {
                model.Id = (int)Guid.NewGuid().GetHashCode(); // Generate a unique integer ID
                _contactUsCollection.InsertOne(model);
                ViewBag.Message = "Thank you for contacting us. We will get back to you soon.";
                return View(new ContactUsModel()); // Clear the form by returning a new instance
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult CustomerQueries()
        {
            var customerQueries = _contactUsCollection.Find(_ => true).ToList();
            return View(customerQueries);
        }
    }
}