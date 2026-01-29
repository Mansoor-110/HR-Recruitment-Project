namespace HR_Recruitment.Models
{
    public class FinalDecisionVM
    {
        public int ApplicantVacancyId { get; set; }

        public string ApplicantName { get; set; }

        public string JobTitle { get; set; }

        public DateTime AppliedDate { get; set; }

        public string CurrentStatus { get; set; }
    }
}
