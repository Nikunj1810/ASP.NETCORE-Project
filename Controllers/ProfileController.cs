using Microsoft.AspNetCore.Mvc;
using ASP.netcore_Project.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ASP.netcore_Project.Controllers
{
    public class ProfileController : Controller
    {   
        private readonly IMongoCollection<OrderModel> _orderCollection;

        public ProfileController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _orderCollection = database.GetCollection<OrderModel>("Orders");
        }
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

        public async Task<IActionResult> OrderHistory()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderCollection
                .Find(o => o.UserId == userId)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(string id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderCollection
                .Find(o => o.Id == id && o.UserId == userId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
