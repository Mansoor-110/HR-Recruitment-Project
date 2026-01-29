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
                subject = "Interview Result – Shortlisted";

                body = $@"
<h3>Dear {applicant.FullName},</h3>

<p>We are pleased to inform you that you have been <b>successfully selected by the interviewer</b> 
for the position of <b>{appVacancy.Vacancy?.Title}</b>.</p>

<p>This means you have cleared the interview stage. Your application has now been forwarded to 
the HR department for <b>final approval</b>.</p>

<p>Once the HR approval is completed, you will be contacted with further details regarding 
joining and onboarding.</p>

<p>Thank you for your time and effort.</p>

<p>Best Regards,<br/>
JobFinder Team</p>
";
            }
            else if (result == "Rejected")
            {
                subject = "Interview Result – Update";

                body = $@"
<h3>Dear {applicant.FullName},</h3>

<p>Thank you for attending the interview for the position of 
<b>{appVacancy.Vacancy?.Title}</b>.</p>

<p>After careful consideration, we regret to inform you that you have not been selected 
to proceed further at this time.</p>

<p>We truly appreciate your interest and wish you success in your future endeavors.</p>

<p>Regards,<br/>
JobFinder Team</p>
";
            }


            EmailHelper.Send(user.Email, subject, body);
        }

    }


    }
