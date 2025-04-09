using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ASP.netcore_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly IMongoCollection<CartModel> _cartCollection;
        private readonly IMongoCollection<ProductModel> _productCollection;

        public CartController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _cartCollection = database.GetCollection<CartModel>("Cart");
            _productCollection = database.GetCollection<ProductModel>("Products");
        }

   

        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId, string size = null, int quantity = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                // Get product details
                var product = await _productCollection.Find(p => p.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                    return NotFound("Product not found");

                // Check if the product already exists in the cart with the same size
                var existingCartItem = await _cartCollection.Find(c => 
                    c.UserId == userId && 
                    c.ProductId == productId && 
                    c.Size == size
                ).FirstOrDefaultAsync();

                if (existingCartItem != null)
                {
                    // Update the quantity of the existing item
                    var filter = Builders<CartModel>.Filter.Eq(c => c.Id, existingCartItem.Id);
                    var update = Builders<CartModel>.Update.Inc(c => c.Quantity, quantity);
                    await _cartCollection.UpdateOneAsync(filter, update);
                }
                else
                {
                    // Create a new cart item
                    var cartItem = new CartModel
                    {
                        UserId = userId,
                        ProductId = productId,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl,
                        Size = size,
                        SizeType = "Regular", // You might want to get this from the product
                        Quantity = quantity,
                        Price = product.Price
                    };

                    // Add to cart
                    await _cartCollection.InsertOneAsync(cartItem);
                }

                TempData["SuccessMessage"] = "Item added to cart successfully!";
                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to add item to cart. Please try again.";
                return RedirectToAction("Shop", "Shop");
            }
        }


        // View Cart
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var items = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();
            return View(items);
        }

        // Remove item from cart
        [HttpPost]
        public async Task<IActionResult> Remove(string id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var filter = Builders<CartModel>.Filter.And(
                Builders<CartModel>.Filter.Eq(c => c.Id, id),
                Builders<CartModel>.Filter.Eq(c => c.UserId, userId)
            );

            await _cartCollection.DeleteOneAsync(filter);
            return RedirectToAction("Cart");
        }

        // Update item quantity in cart
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string id, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // Validate quantity
            if (quantity < 1)
                quantity = 1;

            var filter = Builders<CartModel>.Filter.And(
                Builders<CartModel>.Filter.Eq(c => c.Id, id),
                Builders<CartModel>.Filter.Eq(c => c.UserId, userId)
            );

            var update = Builders<CartModel>.Update.Set(c => c.Quantity, quantity);
            await _cartCollection.UpdateOneAsync(filter, update);

            return RedirectToAction("Cart");
        }

    }
}
