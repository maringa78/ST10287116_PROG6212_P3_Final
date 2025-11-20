using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Services;

namespace ST10287116_PROG6212_POE_P2.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _auth;
        public LoginModel(AuthService auth) => _auth = auth;

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        public string? Error { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var user = _auth.ValidateUser(Email, Password);
            if (user == null)
            {
                Error = "Invalid credentials";
                return Page();
            }
            await _auth.SignInAsync(HttpContext, user);
            // Role-based redirect (avoid hitting protected page without role)
            var target = user.Role switch
            {
                UserRole.Lecturer => "/Lecturer/Dashboard",
                UserRole.Coordinator => "/Coordinator/Track",
                UserRole.Manager => "/Manager/Dashboard",
                UserRole.HR => "/HR/User",
                _ => "/"
            };
            return Redirect(returnUrl ?? target);
        }
    }
}