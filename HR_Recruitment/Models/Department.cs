using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
