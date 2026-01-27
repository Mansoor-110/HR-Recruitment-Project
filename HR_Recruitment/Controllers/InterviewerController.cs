using HR_Recruitment.Models;
using HR_Recruitment.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Recruitment.Controllers
{
    public class InterviewerController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public InterviewerController(RecruitmentSystemContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        // -------------------------------
        // SCHEDULED INTERVIEWS DASHBOARD
        // -------------------------------
        public IActionResult ScheduledInterviews()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (!employeeId.HasValue)
            {
                // Optional: add log here
                return RedirectToAction("Login", "Auth");
            }

            int interviewerId = employeeId.Value;


            // Assigned interviews only
            var interviews = (from i in _context.Interviews
                              join av in _context.ApplicantVacancies
                                  on i.ApplicantVacancyId equals av.ApplicantVacancyId
                              join a in _context.Applicants
                                  on av.ApplicantId equals a.ApplicantId
                              join v in _context.Vacancies
                                  on av.VacancyId equals v.VacancyId
                              where i.InterviewerEmployeeId == interviewerId
                              select new InterviewVM
                              {
                                  InterviewId = i.InterviewId,
                                  ApplicantVacancyId = av.ApplicantVacancyId,
                                  ApplicantName = a.FullName,
                                  JobTitle = v.Title,
                                  InterviewDate = i.InterviewDate,
                                  StartTime = i.StartTime,
                                  EndTime = i.EndTime,
                                  Result = i.Result
                              }).ToList();

            return View(interviews);
        }

        // -------------------------------
        // APPROVE INTERVIEW
        // -------------------------------
        public IActionResult Approve(int id)
        {
            UpdateInterview(id, "Selected");
            return RedirectToAction("ScheduledInterviews");
        }

        public IActionResult Reject(int id)
        {
            UpdateInterview(id, "Rejected");
            return RedirectToAction("ScheduledInterviews");
        }



        private void UpdateInterview(int interviewId, string result)
        {
            var interview = _context.Interviews
                .FirstOrDefault(i => i.InterviewId == interviewId);

            if (interview == null) return;

            interview.Result = result;

            var appVacancy = _context.ApplicantVacancies
                .FirstOrDefault(av => av.ApplicantVacancyId == interview.ApplicantVacancyId);

            if (appVacancy != null)
            {
                appVacancy.Status = result; // 👈 Selected / Rejected
            }

            _context.SaveChanges();
        }



    }
}
