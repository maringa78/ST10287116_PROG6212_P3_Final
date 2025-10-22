using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ST10287116_PROG6212_POE_P2.Models;

namespace ST10287116_PROG6212_POE_P2.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    public class DashboardController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index()
        {
            string? userId = HttpContext.Session.GetString("UserId");
            int lecturerId = 1;

            // Fallback to the default user id used on claim creation
            if (string.IsNullOrEmpty(userId))
                userId = "1";

            var claims = _context.Claims
                .AsNoTracking()
                .Where(c => c.UserId == userId || c.LecturerId == lecturerId)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return View(claims);
        }
    }
}
