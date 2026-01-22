using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class Vacancy
{
    public int VacancyId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int DepartmentId { get; set; }

    public int TotalOpenings { get; set; }

    public int FilledOpenings { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? CloseDate { get; set; }

    public int CreatedByEmployeeId { get; set; }

    public virtual ICollection<ApplicantVacancy> ApplicantVacancies { get; set; } = new List<ApplicantVacancy>();

    public virtual Employee CreatedByEmployee { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;
}
