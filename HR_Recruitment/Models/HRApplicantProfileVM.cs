namespace HR_Recruitment.Models
{
    public class HRApplicantProfileVM
    {
        // Applicant Basic Info
        public int ApplicantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Statistics
        public int TotalApplications { get; set; }
        public int TotalInterviews { get; set; }
        public int PendingApplications { get; set; }
        public int SelectedApplications { get; set; }
        public int RejectedApplications { get; set; }

        // All Applications
        public List<ApplicantApplicationVM> Applications { get; set; }

        // All Interviews
        public List<ApplicantInterviewVM> Interviews { get; set; }
    }

    // This class is already created, but including it for reference


    public class ApplicantInterviewVM
    {
        public int InterviewId { get; set; }
        public string JobTitle { get; set; }
        public string InterviewerName { get; set; }
        public DateTime InterviewDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Result { get; set; }
    }
}