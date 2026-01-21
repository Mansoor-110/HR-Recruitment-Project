using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
