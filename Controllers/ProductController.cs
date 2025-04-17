using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using ASP.netcore_Project.Models;
using MongoDB.Bson;
using System.Globalization;

namespace ASP.netcore_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMongoCollection<ProductModel> _productCollection;
        private readonly IMongoCollection<BsonDocument> _counterCollection;

        public ProductController(IWebHostEnvironment environment)
        {
            _environment = environment;

            var client = new MongoClient("mongodb+srv://Nikunj:NikunjG2004@quickcart.dkxso.mongodb.net/?retryWrites=true&w=majority&appName=QuickCart");
            var database = client.GetDatabase("ASP_QuickCartDB");

            _productCollection = database.GetCollection<ProductModel>("Products");
            _counterCollection = database.GetCollection<BsonDocument>("counters");
        }

        private async Task<string> GetNextProductId()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "productId");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
            int seq = counter["seq"].AsInt32;
            return $"PROD-{seq.ToString().PadLeft(4, '0')}";
        }

        public IActionResult AllProducts()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
            {
                return RedirectToAction("AdminLogin");
            }

            var products = _productCollection.Find(_ => true).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            // Initialize a new ProductModel to prevent null reference exceptions in the view
            var model = new ProductModel
            {
                Sizes = new List<SizeModel>(),
                SizeType = "standard",
                ImageUrls = new List<string>()
            };
            return View(model);
        }

        public IActionResult EditProduct(string id)
        {
            var product = _productCollection.Find(p => p.Id == id).FirstOrDefault();
            if (product == null) return NotFound();

            ViewBag.IsEdit = true;
            return View("AddProduct", product); // Use same view for add/edit
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductModel model, List<IFormFile> imageFiles, List<string> ExistingImageUrls)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = !string.IsNullOrEmpty(model.Id);
                return View("AddProduct", model);
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = await GetNextProductId();
                model.CreatedAt = DateTime.UtcNow;
            }

            // Initialize ImageUrls list
            model.ImageUrls = new List<string>();

            // Add existing images that weren't removed
            if (ExistingImageUrls != null)
            {
                model.ImageUrls.AddRange(ExistingImageUrls);
            }

            // Handle new image uploads
            if (imageFiles != null && imageFiles.Count > 0)
            {
                string uploads = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                foreach (var imageFile in imageFiles)
                {
                    if (imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(uploads, fileName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fs);
                        }

                        model.ImageUrls.Add("/uploads/" + fileName);
                    }
                }
            }

            // Calculate stock quantity
            if (model.SizeType == "standard" || model.SizeType == "waist")
            {
                model.StockQuantity = model.Sizes?.Sum(s => s.Quantity) ?? 0;
            }

            var existingProduct = _productCollection.Find(p => p.Id == model.Id).FirstOrDefault();

            if (existingProduct == null)
            {
                await _productCollection.InsertOneAsync(model);
                TempData["Success"] = "Product added successfully!";
            }
            else
            {
                // If no new images uploaded and no existing images specified, keep the existing product's images
                if ((imageFiles == null || !imageFiles.Any()) && (ExistingImageUrls == null || !ExistingImageUrls.Any()))
                {
                    model.ImageUrls = existingProduct.ImageUrls;
                }

                var update = Builders<ProductModel>.Update
                    .Set(p => p.Name, model.Name)
                    .Set(p => p.Description, model.Description)
                    .Set(p => p.Category, model.Category)
                    .Set(p => p.Gender, model.Gender)
                    .Set(p => p.Brand, model.Brand)
                    .Set(p => p.Sku, model.Sku)
                    .Set(p => p.SizeType, model.SizeType)
                    .Set(p => p.Sizes, model.Sizes)
                    .Set(p => p.StockQuantity, model.StockQuantity)
                    .Set(p => p.Price, model.Price)
                    .Set(p => p.OriginalPrice, model.OriginalPrice)
                    .Set(p => p.DiscountPercentage, model.DiscountPercentage)
                    .Set(p => p.IsNewArrival, model.IsNewArrival)
                    .Set(p => p.ImageUrls, model.ImageUrls);

                await _productCollection.UpdateOneAsync(p => p.Id == model.Id, update);
                TempData["Success"] = "Product updated successfully!";
            }

            return RedirectToAction("AllProducts");
        }

        public IActionResult ProductDetail(string id)
        {
            var product = _productCollection.Find(p => p.Id == id).FirstOrDefault();
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var result = await _productCollection.DeleteOneAsync(p => p.Id == id);
                if (result.DeletedCount > 0)
                {
                    return Json(new { success = true, message = "Product deleted successfully!" });
                }
                return Json(new { success = false, message = "Product not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting product. Please try again." });
            }
        }
    }
}
