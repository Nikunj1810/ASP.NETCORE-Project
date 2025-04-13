using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.netcore_Project.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IMongoCollection<CartModel> _cartCollection;
        private readonly IMongoCollection<ProductModel> _productCollection;
        private readonly IMongoCollection<OrderModel> _orderCollection;

        public CheckoutController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _cartCollection = database.GetCollection<CartModel>("Cart");
            _productCollection = database.GetCollection<ProductModel>("Products");
            _orderCollection = database.GetCollection<OrderModel>("Orders");
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();
            if (!cartItems.Any())
                return RedirectToAction("Cart", "Cart");

            return View(cartItems);
        }

        // POST: Checkout/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                // Get all cart items for the user
                var cartItems = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();

                if (!cartItems.Any())
                    return RedirectToAction("Cart", "Cart");

                // Validate shipping info
                var fullName = HttpContext.Request.Form["FullName"];
                var email = HttpContext.Request.Form["Email"];
                var phone = HttpContext.Request.Form["Phone"];
                var address = HttpContext.Request.Form["Address"];
                var city = HttpContext.Request.Form["City"];
                var state = HttpContext.Request.Form["State"];
                var zipCode = HttpContext.Request.Form["ZipCode"];
                
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                    string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || 
                    string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state) || 
                    string.IsNullOrEmpty(zipCode))
                {
                    TempData["ErrorMessage"] = "Please fill all required shipping information fields.";
                    return RedirectToAction("Index");
                }

                // Create order
                var order = new OrderModel
                {
                    UserId = userId,
                    Items = cartItems.Select(c => new OrderItemModel
                    {
                        ProductId = c.ProductId,
                        Name = c.ProductName,
                        Price = (double)c.Price,
                        Size = c.Size,
                        SizeType = c.SizeType,
                        Quantity = c.Quantity,
                        ImageUrl = c.ImageUrl
                    }).ToList(),
                    ShippingInfo = new ShippingInfo
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Address = address,
                        City = city,
                        State = state,
                        ZipCode = zipCode
                    },
                    PaymentMethod = HttpContext.Request.Form["PaymentMethod"],
                    Subtotal = (double)cartItems.Sum(c => c.Price * c.Quantity),
                    DeliveryFee = cartItems.Sum(c => c.Price * c.Quantity) < 500m ? 150.0 : 0.0,
                    OrderTotal = (double)(cartItems.Sum(c => c.Price * c.Quantity) + (cartItems.Sum(c => c.Price * c.Quantity) < 500m ? 150.0m : 0m)),
                    OrderDate = DateTime.Now,
                    Status = "pending"
                };

                await _orderCollection.InsertOneAsync(order);
                
                // Store the order ID in TempData
                TempData["OrderId"] = order.Id;

                // Clear cart
                await _cartCollection.DeleteManyAsync(c => c.UserId == userId);

                return RedirectToAction("OrderConfirmation", "Checkout");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error placing order: " + ex.Message);
                return RedirectToAction("Cart", "Cart");
            }
        }

        // GET: Checkout/OrderConfirmation
        public IActionResult OrderConfirmation()
        {
            // Retrieve the order ID from TempData
            var orderId = TempData["OrderId"]?.ToString();
            
            // Pass the order ID to the view
            ViewBag.OrderId = orderId;
            
            return View();
        }
    }
}