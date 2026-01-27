namespace HR_Recruitment.Models
{
    public class HrDashboardVM
    {
        public int TotalVacancies { get; set; }
        public int OpenVacancies { get; set; }

        public int TotalApplicants { get; set; }
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }
        public int ShortlistedApplications { get; set; }
        public int RejectedApplications { get; set; }

        public List<RecentApplicationVM> RecentApplications { get; set; }
    }
}
