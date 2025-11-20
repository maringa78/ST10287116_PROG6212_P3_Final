using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Services;  

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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _authService.ValidateUser(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            // Session population
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("NameSurname", user.Username); // User has no Name/Surname properties
            HttpContext.Session.SetString("HourlyRate", user.HourlyRate.ToString("F2"));

            // Role-based redirect
            switch (user.Role)
            {
                case UserRole.Lecturer:
                    return RedirectToAction("Index", "Dashboard", new { area = "Lecturer" });
                case UserRole.Coordinator:
                    return RedirectToAction("Index", "Track", new { area = "Coordinator" });
                case UserRole.Manager:
                    return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }   
}
