using Microsoft.AspNetCore.Mvc.RazorPages;
using ST10287116_PROG6212_POE_P2.Services;

namespace ST10287116_PROG6212_POE_P2.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly AuthService _auth;
        public LogoutModel(AuthService auth) => _auth = auth;
        public async Task OnPostAsync()
        {
            await _auth.SignOutAsync(HttpContext);
            Response.Redirect("/Account/Login");
        }
    }
}