using System.Linq;
using ST10287116_PROG6212_POE_P2.Data;
using ST10287116_PROG6212_POE_P2.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Claim = System.Security.Claims.Claim;

namespace ST10287116_PROG6212_POE_P2.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _ctx;
        public AuthService(ApplicationDbContext ctx) => _ctx = ctx;

        public User? ValidateUser(string email, string password) =>
            _ctx.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Session.Clear();
        }

        public async Task SignInAsync(HttpContext context, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}