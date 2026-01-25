using HR_Recruitment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public AdminController(RecruitmentSystemContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddVacancy()
        {
            var departments = _context.Departments
                                  .OrderBy(d => d.DepartmentName)
                                  .ToList();

            // Pass to view as SelectList
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddVacancy(Vacancy model, IFormFile ImagePath)
        {
           
                // Handle image upload
                if (ImagePath != null && ImagePath.Length > 0)
                {
                    var fileName = Path.GetFileName(ImagePath.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/vacancies", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImagePath.CopyTo(stream);
                    }

                    model.ImagePath = fileName; // save filename in DB
                }

                model.Status = "Open";
                model.CreatedDate = DateTime.Now;
                model.FilledOpenings = 0;
                model.CreatedByEmployeeId = 1; // replace with logged-in employee ID

                _context.Vacancies.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Vacancy added successfully!";
                return RedirectToAction("Index","Admin");
            

            // Reload departments if validation fails
            var departments = _context.Departments.OrderBy(d => d.DepartmentName).ToList();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");

            return View(model);
        }

    }
}
