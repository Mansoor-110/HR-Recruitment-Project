using HR_Recruitment.Helpers;
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
            var featuredJobs = _context.Vacancies
                .Include(v => v.Department)
                .Where(v => v.Status == "Open")
                .OrderByDescending(v => v.CreatedDate)
                .Take(5)
                .Select(v => new VacancyDepartmentVM
                {
                    VacancyId = v.VacancyId,
                    Title = v.Title,
                    CreatedDate = v.CreatedDate,
                    ImagePath = v.ImagePath,
                    DepartmentName = v.Department.DepartmentName
                })
                .ToList();

            return View(featuredJobs);
        }

        public IActionResult FindJob(
       string? keyword,
       int? departmentId,
       string? status
   )
        {
            var query = _context.Vacancies
                .Include(v => v.Department)
                .AsQueryable();

            // 🔍 Keyword filter
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(v =>
                    v.Title.Contains(keyword) ||
                    v.Description.Contains(keyword)
                );
            }

            // 🏢 Department filter
            if (departmentId != null)
            {
                query = query.Where(v => v.DepartmentId == departmentId);
            }

            // 📌 Status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(v => v.Status == status);
            }

            var jobs = query
                .OrderByDescending(v => v.CreatedDate)
                .Select(v => new VacancyDepartmentVM
                {
                    VacancyId = v.VacancyId,
                    Title = v.Title,
                    Description = v.Description,
                    CreatedDate = v.CreatedDate,
                    CloseDate = v.CloseDate,
                    Status = v.Status,
                    ImagePath = v.ImagePath,
                    DepartmentName = v.Department.DepartmentName
                })
                .ToList();

            ViewBag.Departments = _context.Departments.ToList();

            return View(jobs);
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

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

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

            var vacancy = _context.Vacancies.FirstOrDefault(v => v.VacancyId == VacancyId);

            string subject = "Application Received – JobFinder";

            string body = $@"
        <h3>Hello {applicant.FullName},</h3>
        <p>
            You have successfully applied for the position of 
            <b>{vacancy?.Title}</b>.
        </p>
        <p>
            Our HR team will review your application and contact you
            if your profile matches our requirements.
        </p>
        <br/>
        <p>
            Regards,<br/>
            <b>JobFinder HR Team</b>
        </p>
    ";

            EmailHelper.Send(
                user.Email,
                subject,
                body
            );



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

        [HttpPost]
        public IActionResult SubmitComplaint(Complaint complaint)
        {
            if (ModelState.IsValid)
            {
                _context.Complaints.Add(complaint);
                _context.SaveChanges();

                TempData["Success"] = "Your complaint has been submitted successfully!";
                return RedirectToAction("Contact");
            }

            return View("Contact");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
