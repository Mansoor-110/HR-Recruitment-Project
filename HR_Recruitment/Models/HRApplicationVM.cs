using System;

namespace HR_Recruitment.Models
{
    public class HRApplicationVM
    {
        public int ApplicantId { get; set; }
        public int ApplicantVacancyId { get; set; }
        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; }
    }


}
