namespace HR_Recruitment.Models
{
    public class InterviewerDashboardVM
    {
        // Interview Statistics
        public int TotalInterviews { get; set; }
        public int PendingInterviews { get; set; }
        public int CompletedInterviews { get; set; }
        public int ApprovedCandidates { get; set; }
        public int RejectedCandidates { get; set; }

        // Today's Interviews
        public int TodayInterviews { get; set; }

        // Recent Interviews (Last 5)
        public List<RecentInterviewVM> RecentInterviews { get; set; }
    }

    public class RecentInterviewVM
    {
        public int InterviewId { get; set; }
        public int ApplicantVacancyId { get; set; }
        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }
        public DateTime InterviewDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Result { get; set; }
    }
}
