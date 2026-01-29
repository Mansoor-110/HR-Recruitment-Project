using HR_Recruitment.Helpers;
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
            var vm = new HrDashboardVM();

            // 🔹 Vacancies
            vm.TotalVacancies = _context.Vacancies.Count();

            vm.OpenVacancies = _context.Vacancies
                .Count(v => v.Status == "Open");

            // 🔹 Applicants
            vm.TotalApplicants = _context.Applicants.Count();

            // 🔹 Applications
            vm.TotalApplications = _context.ApplicantVacancies.Count();

            vm.PendingApplications = _context.ApplicantVacancies
                .Count(av => av.Status == "Applied");

            vm.ShortlistedApplications = _context.ApplicantVacancies
                .Count(av => av.Status == "Selected");

            vm.RejectedApplications = _context.ApplicantVacancies
                .Count(av => av.Status == "Rejected");

            // 🔹 Recent Applications (Last 5)
            vm.RecentApplications = _context.ApplicantVacancies
                .Include(av => av.Applicant)
                .Include(av => av.Vacancy)
                    .ThenInclude(v => v.Department)
                .OrderByDescending(av => av.AppliedDate)
                .Take(5)
                .Select(av => new RecentApplicationVM
                {
                    ApplicantName = av.Applicant.FullName,
                    JobTitle = av.Vacancy.Title,
                    DepartmentName = av.Vacancy.Department.DepartmentName,
                    AppliedDate = av.AppliedDate,
                    ApplicationStatus = av.Status
                })
                .ToList();

            return View(vm);
        }

        public IActionResult Users()
        {
            var applicants = _context.Applicants
                 .Include(a => a.User)
                 .Include(a => a.ApplicantVacancies)
                 .OrderByDescending(a => a.CreatedDate)
                 .ToList();

            return View(applicants);
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
            return RedirectToAction("Index", "Admin");


            // Reload departments if validation fails
            var departments = _context.Departments.OrderBy(d => d.DepartmentName).ToList();
            ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");

            return View(model);
        }

        public IActionResult Applications()
        {
            var data = (from av in _context.ApplicantVacancies
                        join a in _context.Applicants on av.ApplicantId equals a.ApplicantId
                        join v in _context.Vacancies on av.VacancyId equals v.VacancyId
                        select new HRApplicationVM
                        {
                            ApplicantVacancyId = av.ApplicantVacancyId,
                            ApplicantId = av.ApplicantId,
                            ApplicantName = a.FullName,
                            JobTitle = v.Title,
                            AppliedDate = av.AppliedDate,
                            Status = av.Status
                        }).ToList();

            return View(data);
        }


        public IActionResult ScheduleInterview(int id)
        {
            ViewBag.Interviewers = _context.Employees
                .Where(e => e.User.RoleId == 3) // Interviewer
                .ToList();

            return View(new ScheduleInterviewVM { ApplicantVacancyId = id });
        }

        [HttpPost]
        public IActionResult ScheduleInterview(ScheduleInterviewVM vm)
        {
            var interview = new Interview
            {
                ApplicantVacancyId = vm.ApplicantVacancyId,
                InterviewerEmployeeId = vm.InterviewerEmployeeId,
                InterviewDate = vm.InterviewDate,
                StartTime = vm.StartTime,
                EndTime = vm.EndTime,
                Result = "Pending"
            };

            _context.Interviews.Add(interview);
            _context.SaveChanges();

            // 1. Get ApplicantVacancy by Id
            var applicantVacancy = _context.ApplicantVacancies
                .Include(av => av.Applicant)
                .FirstOrDefault(av => av.ApplicantVacancyId == vm.ApplicantVacancyId);

            if (applicantVacancy == null)
            {
                // handle error - e.g. return NotFound or error message
            }

            var applicant = applicantVacancy.Applicant;

            // 2. Get user from applicant.UserId
            var user = _context.Users.FirstOrDefault(u => u.UserId == applicant.UserId);

            // Format date and day
            string interviewDate = interview.InterviewDate.ToString("dd MMM yyyy");
            string interviewDay = interview.InterviewDate.ToString("dddd");

            // Subject and body
            string subject = "Interview Scheduled";

            string body = $@"
<h3>Hello {applicant.FullName},</h3>
<p>Your interview has been scheduled successfully.</p>
<p><b>Date:</b> {interviewDate}<br/>
<b>Day:</b> {interviewDay}<br/>
<b>Time:</b> {interview.StartTime}</p>
<p>Please be available at the scheduled time.</p>
<p>Regards,<br/>JobFinder HR Team</p>
";

            // Send email
            EmailHelper.Send(user.Email, subject, body);



            return RedirectToAction("Applications"); // HR back to list
        }


        public IActionResult FinalDecision()
        {
            var data = _context.ApplicantVacancies
                .Where(x => x.Status == "InterviewApproved"
                         || x.Status == "InterviewRejected")
                .ToList();

            return View(data);
        }

        public IActionResult FinalApprove(int id)
        {
            UpdateFinal(id, "HRApproved");
            return RedirectToAction("FinalDecision");
        }

        public IActionResult FinalReject(int id)
        {
            UpdateFinal(id, "HRRejected");
            return RedirectToAction("FinalDecision");
        }

        private void UpdateFinal(int id, string status)
        {
            var app = _context.ApplicantVacancies.Find(id);
            app.Status = status;
            _context.SaveChanges();
        }

        public IActionResult ComplaintsList()
        {
            var complaints = _context.Complaints
                                .OrderByDescending(c => c.CreatedAt)
                                .ToList();
            return View(complaints);
        }


        [HttpPost]
        public IActionResult DeleteComplaint(int id)
        {
            var complaint = _context.Complaints.Find(id);
            if (complaint != null)
            {
                _context.Complaints.Remove(complaint);
                _context.SaveChanges();
            }
            return RedirectToAction("ComplaintsList");
        }
    

    public IActionResult Details(int id)
        {
            // Get the applicant with all related data
            var applicant = _context.Applicants
                .Include(a => a.User)
                .FirstOrDefault(a => a.ApplicantId == id);

            if (applicant == null)
            {
                return NotFound();
            }

            // Get all applications for this applicant
            var allApplications = _context.ApplicantVacancies
                .Include(av => av.Vacancy)
                    .ThenInclude(v => v.Department)
                .Where(av => av.ApplicantId == applicant.ApplicantId)
                .OrderByDescending(av => av.AppliedDate)
                .Select(av => new ApplicantApplicationVM
                {
                    JobTitle = av.Vacancy.Title,
                    DepartmentName = av.Vacancy.Department.DepartmentName,
                    AppliedDate = av.AppliedDate,
                    Status = av.Status
                })
                .ToList();

            // Get all interviews for this applicant
            var allInterviews = (from i in _context.Interviews
                                 join av in _context.ApplicantVacancies
                                     on i.ApplicantVacancyId equals av.ApplicantVacancyId
                                 join e in _context.Employees
                                     on i.InterviewerEmployeeId equals e.EmployeeId
                                 join v in _context.Vacancies
                                     on av.VacancyId equals v.VacancyId
                                 where av.ApplicantId == applicant.ApplicantId
                                 orderby i.InterviewDate descending
                                 select new ApplicantInterviewVM
                                 {
                                     InterviewId = i.InterviewId,
                                     JobTitle = v.Title,
                                     InterviewerName = e.FullName,
                                     InterviewDate = i.InterviewDate.ToDateTime(TimeOnly.MinValue),
                                     StartTime = i.StartTime.ToTimeSpan(),
                                     EndTime = i.EndTime.ToTimeSpan(),
                                     Result = i.Result
                                 })
                                 .ToList();

            // Create the view model
            var profileVM = new HRApplicantProfileVM
            {
                ApplicantId = applicant.ApplicantId,
                FullName = applicant.FullName,
                Email = applicant.User?.Email ?? "N/A",
                UserId = applicant.UserId,
                Status = applicant.Status,
                CreatedDate = applicant.CreatedDate,

                // Statistics
                TotalApplications = allApplications.Count,
                TotalInterviews = allInterviews.Count,
                PendingApplications = allApplications.Count(a => a.Status == "Applied"),
                SelectedApplications = allApplications.Count(a => a.Status == "Selected"),
                RejectedApplications = allApplications.Count(a => a.Status == "Rejected"),

                // All Applications
                Applications = allApplications,

                // All Interviews
                Interviews = allInterviews
            };

            return View(profileVM);
        }

    } 
}
