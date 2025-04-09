using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryModel CategoryObj = new();

        // 🔹 GET: Category Manager Page
        public IActionResult CategoryManager()
        {
            var model = new CategoryModel
            {
                Categories = CategoryObj.GetAllCategories()
            };
            return View(model);
        }

        // 🔹 POST: Add New Category
        [HttpPost]
        public IActionResult CategoryManager(CategoryModel Cat)
        {
            if (ModelState.IsValid)
            {
                bool res = CategoryObj.ADD(Cat);
                if (res)
                {
                    TempData["msg"] = "✅ Category added successfully";
                }
                else
                {
                    TempData["msg"] = "⚠️ Category already exists or failed to add.";
                }

                Cat = new CategoryModel
                {
                    Categories = CategoryObj.GetAllCategories()
                };
                return View(Cat);
            }

            Cat.Categories = CategoryObj.GetAllCategories();
            return View(Cat);
        }

        // 🔹 POST: Delete Category by Name
        [HttpPost]
        public IActionResult DeleteCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                bool result = CategoryObj.DeleteCategory(categoryName);
                TempData["msg"] = result ? "✅ Category deleted successfully" : "❌ Failed to delete category";
            }

            return RedirectToAction(nameof(CategoryManager));
        }
    }
}
