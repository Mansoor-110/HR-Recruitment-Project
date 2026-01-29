namespace HR_Recruitment.Models
{
    public class ApplicantProfileVM
    {
        // Applicant Basic Info
        public int ApplicantVacancyId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }





        public DateTime CreatedDate { get; set; }

        // Current Application Info
        public string CurrentJobTitle { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentApplicationStatus { get; set; }
        public DateTime CurrentAppliedDate { get; set; }

        // Statistics
        public int TotalApplications { get; set; }

        // All Applications
        public List<ApplicantApplicationVM> Applications { get; set; }
    }

    public class ApplicantApplicationVM
    {
        public string JobTitle { get; set; }
        public string DepartmentName { get; set; }
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; }
    }

}
