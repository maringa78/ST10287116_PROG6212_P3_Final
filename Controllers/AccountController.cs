using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ST10287116_PROG6212_POE_P2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _authService.ValidateUser(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            // Session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("NameSurname", $"{user.Name} {user.Surname}");
            HttpContext.Session.SetString("HourlyRate", user.HourlyRate.ToString("F2"));

            // Cookie principal
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
                new System.Security.Claims.Claim(ClaimTypes.Role, user.Role.ToString()),
                new System.Security.Claims.Claim("HourlyRate", user.HourlyRate.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return user.Role switch
            {
                UserRole.Lecturer => RedirectToAction("Index", "Dashboard", new { area = "Lecturer" }),
                UserRole.Coordinator => RedirectToAction("Index", "Track", new { area = "Coordinator" }),
                UserRole.Manager => RedirectToAction("Index", "Dashboard", new { area = "Manager" }),
                UserRole.HR => RedirectToAction("Index", "User", new { area = "HR" }),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
