using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class ApplicantVacancy
{
    public int ApplicantVacancyId { get; set; }

    public int ApplicantId { get; set; }

    public int VacancyId { get; set; }

    public DateTime AppliedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Applicant Applicant { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual Vacancy Vacancy { get; set; } = null!;
}
