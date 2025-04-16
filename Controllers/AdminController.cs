using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ASP.netcore_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminModel AdminObj = new();
        private readonly IMongoCollection<OrderModel> _orderCollection;
        private readonly IMongoCollection<ContactUsModel> _contactUsCollection;
        private readonly IMongoCollection<AccountModel> _userCollection;
        private readonly IMongoCollection<ProductModel> _productCollection;

        public AdminController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _orderCollection = database.GetCollection<OrderModel>("Orders");
            _contactUsCollection = database.GetCollection<ContactUsModel>("ContactUs");
            _userCollection = database.GetCollection<AccountModel>("Users");
            _productCollection = database.GetCollection<ProductModel>("Products");
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            var totalUsers = await _userCollection.CountDocumentsAsync(FilterDefinition<AccountModel>.Empty);
            var totalOrders = await _orderCollection.CountDocumentsAsync(FilterDefinition<OrderModel>.Empty);
            var totalProducts = await _productCollection.CountDocumentsAsync(FilterDefinition<ProductModel>.Empty);
            var deliveredOrders = await _orderCollection.CountDocumentsAsync(o => o.Status == "delivered");

            var stats = new AdminDashboardModel
            {
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                DeliveredOrders = deliveredOrders
            };

            return View(stats);
        }

        // GET: /Admin/AdminLogin
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: /Admin/AdminLogin
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

                ModelState.AddModelError(string.Empty, "Invalid username or password!");
            }

            return View(model);
        }

        // GET: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutSuccess"] = "You have logged out successfully!";
            return RedirectToAction("AdminLogin");
        }

        // GET: /Admin/AdminProfile
        public IActionResult AdminProfile()
        {
            string? adminUsername = HttpContext.Session.GetString("AdminUser");

            if (string.IsNullOrEmpty(adminUsername))
                return RedirectToAction("AdminLogin");

            var admin = new AdminModel { UserName = adminUsername };
            return View(admin);
        }

        private bool IsAdminLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser"));
        }

        // GET: /Admin/OrderList
        public async Task<IActionResult> OrderList(string status = null)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            var filterBuilder = Builders<OrderModel>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter = filterBuilder.Eq(o => o.Status, status);
            }

            var orders = await _orderCollection.Find(filter)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.CurrentStatus = status;

            return View(orders);
        }

        // POST: /Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            if (request?.OrderIds == null || !request.OrderIds.Any() || string.IsNullOrEmpty(request.Status))
                return BadRequest(new { success = false, message = "Order IDs and status are required" });

            try
            {
                var validStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
                if (!validStatuses.Contains(request.Status.ToLower()))
                    return BadRequest(new { success = false, message = "Invalid status value" });

                var filter = Builders<OrderModel>.Filter.In(o => o.Id, request.OrderIds);
                var update = Builders<OrderModel>.Update
                    .Set(o => o.Status, request.Status)
                    .Set(o => o.LastUpdated, DateTime.UtcNow);

                var result = await _orderCollection.UpdateManyAsync(filter, update);

                if (result.ModifiedCount > 0)
                {
                    TempData["SuccessMessage"] = $"{result.ModifiedCount} order(s) status updated successfully";
                    return Json(new { success = true, message = $"{result.ModifiedCount} order(s) status updated successfully" });
                }
                else if (result.MatchedCount > 0)
                {
                    return Json(new { success = true, message = "Orders were already in the requested status" });
                }

                return NotFound(new { success = false, message = "No matching orders found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating orders" });
            }
        }

        public class UpdateOrderStatusRequest
        {
            public List<string> OrderIds { get; set; }
            public string Status { get; set; }
        }

        // GET: /Admin/OrderDetail
        public async Task<IActionResult> OrderDetail(string orderId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, orderId);
            var order = await _orderCollection.Find(filter).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: /Admin/CustomerQueries
        public async Task<IActionResult> CustomerQueries()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            var customerQueries = await _contactUsCollection.Find(_ => true)
                .SortByDescending(q => q.Id)
                .ToListAsync();

            return View(customerQueries);
        }

        // POST: /Admin/DeleteQuery
        [HttpPost]
        public async Task<IActionResult> DeleteQuery(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            var filter = Builders<ContactUsModel>.Filter.Eq("Id", id);
            await _contactUsCollection.DeleteOneAsync(filter);

            TempData["SuccessMessage"] = "Query deleted successfully!";
            return RedirectToAction("CustomerQueries");
        }
    }
}
