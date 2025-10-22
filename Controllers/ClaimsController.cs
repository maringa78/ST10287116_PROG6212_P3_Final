using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;

namespace ST10287116_PROG6212_POE_P2.Controllers
{
    public class ClaimsController(ClaimService claimService) : Controller
    {
        private readonly ClaimService _claimService = claimService;

        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(Claim model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ClaimDate = model.ClaimDate == default ? DateTime.Now : model.ClaimDate;
            model.Status = ClaimStatus.Pending;

            var userId = HttpContext.Session.GetString("UserId");
            model.UserId = string.IsNullOrEmpty(userId) ? "1" : userId;

            // Ensure lecturer filter shows these claims
            model.LecturerId = 1;

            _claimService.CreateClaim(model);

            return RedirectToAction("Index", "Dashboard", new { area = "Lecturer" });
        }
    }
}