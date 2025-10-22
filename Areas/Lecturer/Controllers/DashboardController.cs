using Microsoft.AspNetCore.Mvc;
namespace ST10287116_PROG6212_POE_P2.Areas.Lecturer.Controllers;


[Area("Lecturer")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Load the lecturer's claims (change the LecturerId if needed)
        int lecturerId = 1;

        var claims = _context.Claims
            .Where(c => c.LecturerId == lecturerId)
            .OrderByDescending(c => c.ClaimDate)
            .ToList();

        return View(claims);
    }
}
