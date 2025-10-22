using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Diagnostics;
using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Services;  
namespace ST10287116_PROG6212_POE_P2.Controllers
{
    public class AccountController(AuthService authService, ILogger<AccountController> logger) : Controller 
    {
        private readonly AuthService _authService = authService;
        private readonly ILogger<AccountController> _logger = logger;

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _authService.ValidateUser(model.Email, model.Password);
                    if (user != null)
                    {
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("Role", user.Role.ToString());
                        return RedirectToAction("Index", user.Role.ToString() == "Lecturer" ? "Dashboard" :
                                                 user.Role.ToString() == "Coordinator" ? "Track" : "Dashboard",
                                                 new { area = user.Role.ToString() });
                    }
                    ModelState.AddModelError(string.Empty, "Invalid credentials");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during login for {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public class UserLogin
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
