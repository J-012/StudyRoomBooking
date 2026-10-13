using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;
using System.Security.Claims;

namespace StudyRoomBooking.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudyRoomDbContext _context;
        private readonly PasswordHasher<Student> _passwordHasher = new();

        public AccountController(StudyRoomDbContext context)
        {
            _context = context;
        }

        // LOGIN

        [HttpGet]
        public IActionResult Login(bool registered = false)
        {
            // Kommer fra register siden, viser suksessmelding
            if (registered)
            {
                ViewBag.Success = "Account created successfully. You can now log in.";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string studentid, string password)
        {
            if (string.IsNullOrWhiteSpace(studentid) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Enter your StudentID and password.";
                return View();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentid.Trim());

            if (student == null ||
                _passwordHasher.VerifyHashedPassword(
                    student, student.PasswordHash, password)
                    == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, student.StudentId.ToString()),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimTypes.Name, student.Name)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        // REGISTER

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string studentid,
            string name,
            string password,
            string confirmPassword)
        {
            studentid = studentid?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(studentid))
            {
                ViewBag.Error = "StudentID is required.";
                return View();
            }

            if (studentid.Length < 3 || studentid.Length > 50)
            {
                ViewBag.Error = "StudentID must be between 3 and 50 characters.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Name is required.";
                return View();
            }
    
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                ViewBag.Error = "Password must be at least 8 characters.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            bool studentidExists = await _context.Students
                .AnyAsync(s => s.StudentId == studentid);

            if (studentidExists)
            {
                ViewBag.Error = "This StudentID is already taken.";
                return View();
            }

            var user = new Student
            {
                StudentId = studentid,
                Name = name
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Students.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", new { registered = true });
        }

        // LOGOUT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // ACCESS DENIED

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
