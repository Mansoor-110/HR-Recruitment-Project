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
        
            [HttpPost]
        public IActionResult Apply(int VacancyId)
        {
            // 1️⃣ Login check
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            // 2️⃣ Applicant fetch
            var applicant = _context.Applicants
                .FirstOrDefault(a => a.UserId == userId);

            if (applicant == null)
                return RedirectToAction("Login", "Auth");

            // 3️⃣ Duplicate apply check
            bool alreadyApplied = _context.ApplicantVacancies
                .Any(av => av.ApplicantId == applicant.ApplicantId && av.VacancyId == VacancyId);

            if (alreadyApplied)
            {
                TempData["Error"] = "You have already applied for this vacancy.";
                return RedirectToAction("Profile", "Home", new { id = VacancyId });
            }

            // 4️⃣ Apply Vacancy
            ApplicantVacancy apply = new ApplicantVacancy
            {
                ApplicantId = applicant.ApplicantId,
                VacancyId = VacancyId,
                Status = "Applied",
                AppliedDate = DateTime.Now
            };

            _context.ApplicantVacancies.Add(apply);
            _context.SaveChanges();

            // 5️⃣ Success
            TempData["Success"] = "Application submitted successfully!";
            return RedirectToAction("Profile");
        }

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var applicant = _context.Applicants
       .FirstOrDefault(a => a.UserId == userId);

            var applications = _context.ApplicantVacancies
                .Where(av => av.ApplicantId == applicant.ApplicantId)
                .Include(av => av.Vacancy)
                .ThenInclude(v => v.Department)
                .Select(av => new MyApplicationVM
                {
                    JobTitle = av.Vacancy.Title,
                    DepartmentName = av.Vacancy.Department.DepartmentName,
                    AppliedDate = av.AppliedDate,
                    ApplicationStatus = av.Status,
                    VacancyStatus = av.Vacancy.Status
                })
                .ToList();

            return View(applications);
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
