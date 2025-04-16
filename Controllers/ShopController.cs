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

        public IActionResult Shop(string gender = null, string category = null)
        {
            var filterBuilder = Builders<ProductModel>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(gender))
            {
                filter = filterBuilder.Eq(p => p.Gender, gender);
            }

            if (!string.IsNullOrEmpty(category))
            {
                var categoryFilter = filterBuilder.Eq(p => p.Category, category);
                filter = filter == filterBuilder.Empty
                    ? categoryFilter
                    : filterBuilder.And(filter, categoryFilter);
            }

            var products = _productCollection.Find(filter).ToList();
            
            // Pass selected filters to the view
            ViewBag.SelectedGender = gender;
            ViewBag.SelectedCategory = category;

            // Get all unique categories for the filter
            var allCategories = _productCollection.Distinct<string>("Category", filterBuilder.Empty).ToList();
            ViewBag.Categories = allCategories;

            return View(products);
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
