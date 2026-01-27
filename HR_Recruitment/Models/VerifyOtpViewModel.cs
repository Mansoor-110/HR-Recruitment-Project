using System;
using System.Collections.Generic;

namespace HR_Recruitment.Models
{
    public class VerifyOtpViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }

    }
}
