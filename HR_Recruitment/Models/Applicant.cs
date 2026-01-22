using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class Applicant
{
    public int ApplicantId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantVacancy> ApplicantVacancies { get; set; } = new List<ApplicantVacancy>();

    public virtual User User { get; set; } = null!;
}
