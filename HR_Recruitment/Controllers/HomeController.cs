using HR_Recruitment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HR_Recruitment.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public HomeController(RecruitmentSystemContext context) 
        {
            this._context = context;
        }


        public IActionResult Index()
        {
          

            return View();
        }
        public IActionResult FindJob()
        {
            var data = _context.Vacancies
           .Include(v => v.Department)
           .Select(v => new VacancyDepartmentVM
           {
               VacancyId = v.VacancyId,
               Title = v.Title,
               Description = v.Description,
               TotalOpenings = v.TotalOpenings,
               FilledOpenings = v.FilledOpenings,
               Status = v.Status,
               CreatedDate = v.CreatedDate,
               CloseDate = v.CloseDate,
               ImagePath = v.ImagePath,

               DepartmentId = v.Department.DepartmentId,
               DepartmentName = v.Department.DepartmentName
           })
           .ToList();

            return View(data);

        }
        public IActionResult JobDetails(int id)
        {
            var job = _context.Vacancies
           .Include(v => v.Department)
           .Where(v => v.VacancyId == id)
           .Select(v => new VacancyDepartmentVM
           {
               VacancyId = v.VacancyId,
               Title = v.Title,
               Description = v.Description,
               TotalOpenings = v.TotalOpenings,
               FilledOpenings = v.FilledOpenings,
               Status = v.Status,
               CreatedDate = v.CreatedDate,
               CloseDate = v.CloseDate,
               ImagePath = v.ImagePath,

               DepartmentId = v.Department.DepartmentId,
               DepartmentName = v.Department.DepartmentName
           })
           .FirstOrDefault();

            if (job == null)
                return NotFound();

            return View(job);
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
