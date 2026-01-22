using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class Interview
{
    public int InterviewId { get; set; }

    public int ApplicantVacancyId { get; set; }

    public int InterviewerEmployeeId { get; set; }

    public DateOnly InterviewDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Result { get; set; } = null!;

    public virtual ApplicantVacancy ApplicantVacancy { get; set; } = null!;

    public virtual Employee InterviewerEmployee { get; set; } = null!;
}
