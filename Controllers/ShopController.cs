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
                new ProductModel { Id = 2, Name = "Cool Jeans", Price = 59.99m, ImageUrl = "jeans.jpg", Category = "Jeans" },
                new ProductModel { Id = 3, Name = "Stylish Hat", Price = 19.99m, ImageUrl = "hat.jpg", Category = "Accessories" },
                new ProductModel { Id = 4, Name = "Elegant Dress", Price = 79.99m, ImageUrl = "dress.jpg", Category = "Dresses" },
                new ProductModel { Id = 5, Name = "Sporty Shoes", Price = 99.99m, ImageUrl = "shoes.jpg", Category = "Shoes" },
                new ProductModel { Id = 6, Name = "Casual Shirt", Price = 39.99m, ImageUrl = "shirt.jpg", Category = "Shirts" }
            };

            return View(products);
        }
    }
}
