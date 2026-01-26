namespace HR_Recruitment.Models
{
    public class MyApplicationVM
    {
    
    public string JobTitle { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public DateTime AppliedDate { get; set; }
    public string ApplicationStatus { get; set; } = null!;
    public string VacancyStatus { get; set; } = null!;
    }
}
