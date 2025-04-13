using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;
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
        
        public AdminController()
            {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _orderCollection = database.GetCollection<OrderModel>("Orders");
            _contactUsCollection = database.GetCollection<ContactUsModel>("ContactUs");
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

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");

            return View();
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
            
            // Apply status filter if provided
            if (!string.IsNullOrEmpty(status))
            {
                filter = filterBuilder.Eq(o => o.Status, status);
            }
            
            // Get all orders with optional filter
            var orders = await _orderCollection.Find(filter)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();
                
            // Pass status for filter dropdown
            ViewBag.CurrentStatus = status;
            
            return View(orders);
        }
        
        // POST: /Admin/UpdateOrderStatus
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string status)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin");
                
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
                return BadRequest("Order ID and status are required");
                
            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<OrderModel>.Update.Set(o => o.Status, status);
            
            var result = await _orderCollection.UpdateOneAsync(filter, update);
            
            if (result.ModifiedCount > 0)
            {
                TempData["SuccessMessage"] = "Order status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update order status";
            }
            
            return RedirectToAction("OrderList");
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
