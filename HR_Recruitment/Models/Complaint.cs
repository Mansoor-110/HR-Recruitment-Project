namespace HR_Recruitment.Models
{
    public partial class Complaint
    {
        public int ComplaintId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
