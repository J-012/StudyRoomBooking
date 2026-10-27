using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // LOGIN
        [HttpGet]
        public IActionResult Login(bool registered = false)
        {
            // Redirect authenticated users to the home page.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // Show a success message after registration.
            if (registered)
            {
                ViewBag.Success = "Account created successfully. You can now log in.";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userId, string password)
        {
            var user = await _userService.AuthenticateAsync(userId, password);

            if (user is null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

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
            string userId,
            string name,
            string email,
            string password,
            string confirmPassword
        )
        {
            var result = await _userService.RegisterAsync(userId, name, password, email, confirmPassword);

            if (!result.Succeeded)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            return RedirectToAction("Login", new { registered = true });
        }

        // LOGOUT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
