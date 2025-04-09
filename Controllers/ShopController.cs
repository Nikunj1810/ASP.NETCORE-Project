using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;

namespace ASP.netcore_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly IMongoCollection<ProductModel> _productCollection;

        public ShopController()
        {
            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");
            _productCollection = database.GetCollection<ProductModel>("Products");
        }

        public IActionResult Shop()
        {
            var products = _productCollection.Find(_ => true).ToList(); // Get all products from MongoDB
            return View(products); // Pass the list to the view
        }

        public IActionResult ProductDetail(string id)
        {
            var product = _productCollection.Find(p => p.Id == id).FirstOrDefault();
            if (product == null)
                return NotFound();

            return View(product); // Render ProductDetail.cshtml with product info
        }
    }
}
