using HR_Recruitment.Helpers;
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


            var applicant = _context.Applicants
       .FirstOrDefault(a => a.ApplicantId == appVacancy.ApplicantId);

            if (applicant == null) return;

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == applicant.UserId);

            if (user == null) return;

            string subject = "";
            string body = "";

            if (result == "Selected")
            {
                subject = "Interview Result – Congratulations!";
                body = $@"
            <h3>Dear {applicant.FullName},</h3>
            <p>Congratulations! You have been <b>selected</b> for the position <b>{appVacancy.Vacancy?.Title}</b>.</p>
            <p>Our HR team will contact you for the next steps.</p>
            <p>Best Regards,<br/>JobFinder Team</p>
        ";
            }
            else if (result == "Rejected")
            {
                subject = "Interview Result – Update";
                body = $@"
            <h3>Dear {applicant.FullName},</h3>
            <p>Thank you for attending the interview for <b>{appVacancy.Vacancy?.Title}</b>.</p>
            <p>Unfortunately, you have not been selected for this position.</p>
            <p>We wish you all the best for your future endeavors.</p>
            <p>Regards,<br/>JobFinder Team</p>
        ";
            }
            else
            {
                // If result is neither Selected nor Rejected, don't send email
                return;
            }

            EmailHelper.Send(user.Email, subject, body);
        }

    }


    }
