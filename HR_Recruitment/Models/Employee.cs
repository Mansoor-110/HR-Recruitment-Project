using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
