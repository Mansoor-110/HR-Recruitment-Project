namespace HR_Recruitment.Models
{
    public class VacancyDepartmentVM
    {
        // Vacancy fields
        public int VacancyId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalOpenings { get; set; }
        public int FilledOpenings { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? ImagePath { get; set; }

        // Department fields
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

    }
}
