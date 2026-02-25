using Microsoft.AspNetCore.Mvc;
using VotersListProject.Models;
using VotersListProject.Services;

namespace VotersListProject.Controllers
{
    public class VotersController : Controller
    {
        private readonly AddDB _db;
        private readonly EmailService _emailService;

        public VotersController(IConfiguration config)
        {
            _db = new AddDB();
            _emailService = new EmailService(config);
        }

        // ============================
        // REGISTER
        // ============================
        [HttpGet]
        public IActionResult AddVoters()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVoters(VoterModel obj)
        {
            if (ModelState.IsValid)
            {
                obj.Role = "User";
                obj.IsEmailVerified = false;

                Random rnd = new Random();
                obj.EmailOTP = rnd.Next(100000, 999999).ToString();
                obj.OTPExpiry = DateTime.Now.AddMinutes(5);

                _db.votertable.Add(obj);
                _db.SaveChanges();

                string body = $"Your OTP for email verification is: <b>{obj.EmailOTP}</b><br/>Valid for 5 minutes.";
                _emailService.SendEmail(obj.Email, "Email Verification OTP", body);

                HttpContext.Session.SetInt32("TempUserId", obj.Id);

                return RedirectToAction("VerifyOTP");
            }

            return View(obj);
        }

        // ============================
        // VERIFY REGISTRATION OTP
        // ============================
        [HttpGet]
        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string otp)
        {
            var userId = HttpContext.Session.GetInt32("TempUserId");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _db.votertable.FirstOrDefault(x => x.Id == userId);

            if (user == null)
                return RedirectToAction("Login");

            if (user.EmailOTP == otp && user.OTPExpiry > DateTime.Now)
            {
                user.IsEmailVerified = true;
                user.EmailOTP = null;
                user.OTPExpiry = null;

                _db.SaveChanges();

                return RedirectToAction("Login");
            }

            ViewBag.Error = "Invalid or Expired OTP";
            return View();
        }

        // ============================
        // LOGIN
        // ============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(VoterModel obj)
        {
            var user = _db.votertable
                .FirstOrDefault(x => x.Email == obj.Email
                                  && x.Password == obj.Password);

            if (user != null)
            {
                if (!user.IsEmailVerified)
                {
                    ViewBag.Error = "Please verify your email before login.";
                    return View();
                }

                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.VoterName);

                if (user.Role == "Admin")
                    return RedirectToAction("ShowData", "DataShow");

                return RedirectToAction("SearchUser");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        // ============================
        // SEARCH USER
        // ============================
        public IActionResult SearchUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchUser(string email)
        {
            var user = _db.votertable.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "User not found";
                return View();
            }

            return View("UserResult", user);
        }

        // ============================
        // LOGOUT
        // ============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ============================
        // CHANGE PASSWORD (Normal)
        // ============================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _db.votertable.FirstOrDefault(x => x.Id == userId);

            if (user.Password != oldPassword)
            {
                ViewBag.Error = "Old password is incorrect";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New passwords do not match";
                return View();
            }

            user.Password = newPassword;
            _db.SaveChanges();

            ViewBag.Success = "Password changed successfully";
            return View();
        }

        // ============================
        // FORGOT PASSWORD (SEND OTP)
        // ============================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _db.votertable.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Email not found";
                return View();
            }

            Random rnd = new Random();
            string otp = rnd.Next(100000, 999999).ToString();

            user.EmailOTP = otp;
            user.OTPExpiry = DateTime.Now.AddMinutes(5);
            _db.SaveChanges();

            string body = $"Your Password Reset OTP is: <b>{otp}</b><br/>Valid for 5 minutes.";
            _emailService.SendEmail(email, "Password Reset OTP", body);

            TempData["ResetEmail"] = email;

            return RedirectToAction("VerifyResetOTP");
        }

        // ============================
        // VERIFY RESET OTP
        // ============================
        [HttpGet]
        public IActionResult VerifyResetOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyResetOTP(string otp, string newPassword, string confirmPassword)
        {
            var email = TempData["ResetEmail"]?.ToString();

            if (email == null)
                return RedirectToAction("ForgotPassword");

            var user = _db.votertable.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("ForgotPassword");

            if (user.EmailOTP != otp || user.OTPExpiry < DateTime.Now)
            {
                ViewBag.Error = "Invalid or expired OTP";
                TempData["ResetEmail"] = email;
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                TempData["ResetEmail"] = email;
                return View();
            }

            user.Password = newPassword;
            user.EmailOTP = null;
            user.OTPExpiry = null;

            _db.SaveChanges();

            TempData["Success"] = "Password reset successful. Please login.";
            return RedirectToAction("Login");
        }
    }
}