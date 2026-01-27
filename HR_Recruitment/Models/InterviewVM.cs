namespace HR_Recruitment.Models
{
    public class InterviewVM
    {
        public int InterviewId { get; set; }
        public int ApplicantVacancyId { get; set; }

        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }

        public DateOnly InterviewDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string Result { get; set; }
    }

}
