using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // For ILogger
using ST10287116_PROG6212_POE_P2.Models;  // For UserLogin (from Models)
using ST10287116_PROG6212_POE_P2.Services;  // For AuthService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

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

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new UserLogin());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            _logger.LogInformation("POST /Account/Login hit. ModelValid={Valid}", ModelState.IsValid);
            if (!ModelState.IsValid) return View(model);

            var user = _authService.ValidateUser(model.Email, model.Password);
            _logger.LogInformation("ValidateUser => {Result}", user == null ? "NULL" : $"{user.Id}:{user.Role}");
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(model);
            }

            // Use System.Security.Claims types (avoid conflict with Models.Claim)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());

            return user.Role switch
            {
                UserRole.HR          => RedirectToAction("Index", "User", new { area = "HR" }),
                UserRole.Lecturer    => RedirectToAction("Index", "Dashboard", new { area = "Lecturer" }),
                UserRole.Coordinator => RedirectToAction("Index", "Track", new { area = "Coordinator" }),
                UserRole.Manager     => RedirectToAction("Index", "Dashboard", new { area = "Manager" }),
                _ => Redirect(returnUrl ?? "/")
            };
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}