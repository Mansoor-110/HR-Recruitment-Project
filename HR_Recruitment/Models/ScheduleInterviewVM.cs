namespace HR_Recruitment.Models
{
    public class ScheduleInterviewVM
    {
        public int ApplicantVacancyId { get; set; }
        public int InterviewerEmployeeId { get; set; }
        public DateOnly InterviewDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }


}
