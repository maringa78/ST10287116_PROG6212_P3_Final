using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;

namespace ST10287116_PROG6212_POE_P2.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class DashboardController : Controller
    {
        private readonly ClaimService _claimService;

        public DashboardController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "1";
            var claims = _claimService.GetUserClaims(userId);
            return View(claims); // View is strongly typed to IEnumerable<Claim>
        }
    }
}
