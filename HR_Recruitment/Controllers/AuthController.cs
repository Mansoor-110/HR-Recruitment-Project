using HR_Recruitment.Helpers;
using HR_Recruitment.Models;
using HR_Recruitment.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace HR_Recruitment.Controllers
{
    public class AuthController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public AuthController(RecruitmentSystemContext context)
        {
            this._context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            if (_context.Users.Any(x => x.Email == obj.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(obj);
            }

            var user = new User
            {
                Email = obj.Email,
                PasswordHash = obj.Password, // ⚠️ later hash
                RoleId = 1,
                IsActive = false,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var applicant = new Applicant
            {
                UserId = user.UserId,
                FullName = obj.FullName,
                Status = "NotInProcess",
                CreatedDate = DateTime.Now
            };

            _context.Applicants.Add(applicant);
            _context.SaveChanges();

            // 🔢 OTP
            var otp = new Random().Next(100000, 999999).ToString();

            var otpRecord = new EmailVerificationOTP
            {
                UserId = user.UserId,
                OTPCode = otp,
                CreatedAt = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMinutes(5),
                IsUsed = false
            };

            _context.EmailVerificationOTPs.Add(otpRecord);
            _context.SaveChanges();

            string subjectFirst = "Verify Your Account";
            string bodyFirst = $@"
    <h1>Hello There!, </h1>
    <p>Thank you for registering with us! Your OTP to verify your account is:</p>
    <h2>{otp}</h2>
    <p>Please enter this code within the next 10 minutes to complete your verification.</p>
    <p>Thanks,<br/>The JobFinder Team</p>
";
            EmailHelper.Send(user.Email, subjectFirst, bodyFirst);


            return RedirectToAction("VerifyOTP", new { userId = user.UserId });
        }


        public IActionResult VerifyOTP(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpGet]
        public IActionResult VerifyOtp(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
                return NotFound();

            var model = new VerifyOtpViewModel
            {
                UserId = userId,
                Email = user.Email
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var record = _context.EmailVerificationOTPs.FirstOrDefault(x =>
                x.UserId == model.UserId &&
                x.OTPCode == model.OtpCode &&
                !x.IsUsed &&
                x.ExpiryDate > DateTime.Now);

            if (record == null)
            {
                ViewData["OtpError"] = "Invalid or expired OTP";
                return View(model);
            }

            record.IsUsed = true;

            var user = _context.Users.Find(model.UserId);
            user.IsActive = true;

            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Register");

            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
                return RedirectToAction("Register");

            // 🔒 Invalidate previous OTPs
            var oldOtps = _context.EmailVerificationOTPs
                .Where(x => x.UserId == user.UserId && !x.IsUsed);

            foreach (var item in oldOtps)
            {
                item.IsUsed = true;
            }

            // 🔢 Generate new OTP
            var otp = new Random().Next(100000, 999999).ToString();

            var otpRecord = new EmailVerificationOTP
            {
                UserId = user.UserId,
                OTPCode = otp,
                IsUsed = false,
                ExpiryDate = DateTime.Now.AddMinutes(5)
            };

            _context.EmailVerificationOTPs.Add(otpRecord);
            _context.SaveChanges();

            string subjectResend = "Your New OTP Code";
            string bodyResend = $@"
    <h1>Hello There!,</h1>
    <p>As requested, here is your new OTP:</p>
    <h2>{otp}</h2>
    <p>Use this OTP to complete your verification. This code will expire in 10 minutes.</p>
    <p>If you did not request this code, please ignore this email.</p>
    <p>Thanks,<br/>The JobFinder Team</p>
";
            EmailHelper.Send(user.Email, subjectResend, bodyResend);


            // 🔁 Back to Verify page
            return RedirectToAction("VerifyOtp", new { userId = user.UserId });
        }




        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User obj)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == obj.Email);

            if (user == null)
            {
                ViewData["AuthError"] = "Invalid Email or Password";
                return View(obj);
            }

            if (!user.IsActive)
            {
                ViewData["AuthError"] = "Please verify your email first";
                return View(obj);
            }

            if (user.PasswordHash != obj.PasswordHash)
            {
                ViewData["AuthError"] = "Password Doesn't Match";
                return View(obj);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);
            HttpContext.Session.SetString("Email", user.Email);

            // Get Employee object from DB
            var employee = _context.Employees.FirstOrDefault(e => e.UserId == user.UserId);
            if (employee != null)
            {
                HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
            }



            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId == 1)
            {
                return RedirectToAction("Index", "Home");

            }
            else if (roleId == 2)
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (roleId == 3)
            {


                return RedirectToAction("Index", "Interviewer");
            }
            return View(obj);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
