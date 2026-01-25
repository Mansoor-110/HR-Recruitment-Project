using HR_Recruitment.Models;
using HR_Recruitment.Models.ViewModels;
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
        
            
                var user = new User
                {
                    Email = obj.Email,
                    PasswordHash = obj.Password,
                    RoleId = 1, // Applicant
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Added Applicant
            var applicant = new Applicant
            {
                UserId = user.UserId,
                FullName = obj.FullName,
                Status = "NotInProcess",
                CreatedDate = DateTime.Now
            };

            _context.Applicants.Add(applicant);
            _context.SaveChanges();

            return RedirectToAction("Login");   

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User obj )
        {
            var user = _context.Users.FirstOrDefault(u => u.Email== obj.Email && u.IsActive == true);
            if (user == null)
            {
                ViewData["AuthError"] = "Invalid Email or Password";
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


            var roleId =HttpContext.Session.GetInt32("RoleId");
            if (roleId == 1)
            {
                return RedirectToAction("Index", "Home");

            }
            else if (roleId == 2) { 
            
                return RedirectToAction("Index", "Admin");
            }else if (roleId == 3) {


                return RedirectToAction("Index", "Admin");
            }
            return View(obj);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }


    }
}
