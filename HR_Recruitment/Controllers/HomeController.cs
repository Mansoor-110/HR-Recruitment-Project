using System.Diagnostics;
using HR_Recruitment.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Recruitment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FindJob()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult Blog_Details()
        {
            return View();
        }
        public IActionResult Job_Details()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }


   
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
