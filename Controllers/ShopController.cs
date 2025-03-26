using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ASP.netcore_Project.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Shop()
        {
            // Dummy product list
            List<ProductModel> products = new List<ProductModel>
            {
                new ProductModel { Id = 1, Name = "Awesome T-Shirt", Price = 29.99m, ImageUrl = "tshirt.jpg", Category = "T-shirts" },
            
            };

            return View(products);
        }
        public IActionResult ProductDetail(int id = 1) {
            // Create a sample product for demonstration
            var product = new ProductModel
            {
                Id = id,
                Name = "Classic Striped Shirt",
                Price = 49.99m,
                ImageUrl = "shirt.jpg",
                Category = "Shirts"
            };
            
            return View(product);
        }
    }
}
