using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Data;
using System.Security.Claims;

namespace ST10287116_PROG6212_POE_P2.Controllers
{
    public class DiagController(ApplicationDbContext ctx) : Controller
    {
        [HttpGet("/diag")]
        public IActionResult Index()
        {
            return Json(new {
                Users = ctx.Users.Count(),
                Authenticated = User.Identity?.IsAuthenticated,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray()
            });
        }
    }
}