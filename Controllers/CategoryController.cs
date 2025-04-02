using ASP.netcore_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class CategoryController : Controller
    {
        CategoryModel CategoryObj = new();
        public IActionResult CategoryManager()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CategoryManager(CategoryModel Cat)
        {
            if (ModelState.IsValid)
            {
                bool res = CategoryObj.ADD(Cat);
                if (res)
                {
                    TempData["msg"] = "Category Added Successfully";
                    return View();
                }
                else
                {
                    TempData["msg"] = "Registration failed. Please try again.";
                }
            }
            return View(Cat);
        }
    }
}
