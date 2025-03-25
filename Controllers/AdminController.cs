using Microsoft.AspNetCore.Mvc;

namespace ASP.netcore_Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }   
     
        
        public IActionResult AllProducts()
        {
            
            return View();
        }
        public IActionResult OrderList()
        {

            return View();
        }

    }
}
