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

        public CheckoutController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _cartCollection = database.GetCollection<CartModel>("Cart");
            _productCollection = database.GetCollection<ProductModel>("Products");
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

                // Here you would typically:
                // 1. Create an order record in your Orders collection
                // 2. Process payment
                // 3. Clear the user's cart

                // For now, just clear the cart
                await _cartCollection.DeleteManyAsync(c => c.UserId == userId);

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("OrderConfirmation");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error placing order: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to place order. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // GET: Checkout/OrderConfirmation
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}