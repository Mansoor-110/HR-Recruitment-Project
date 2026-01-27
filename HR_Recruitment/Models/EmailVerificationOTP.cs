using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR_Recruitment.Models
{
    public class EmailVerificationOTP
    {
        [Key]
        public int OTPId { get; set; }

        public int UserId { get; set; }     
        public User User { get; set; }

        public string OTPCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
