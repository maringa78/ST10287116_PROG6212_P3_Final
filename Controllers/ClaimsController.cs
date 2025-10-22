using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;

namespace ST10287116_PROG6212_POE_P2.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ClaimService _claimService;

        public ClaimsController(ClaimService claimService)
        {
            _claimService = claimService;
        }

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

            // set defaults and user id from session if available
            model.ClaimDate = model.ClaimDate == default ? DateTime.Now : model.ClaimDate;
            model.Status = ClaimStatus.Pending;
            var userId = HttpContext.Session.GetString("UserId");
            model.UserId = string.IsNullOrEmpty(userId) ? "1" : userId;

            _claimService.CreateClaim(model);

            // redirect to lecturer dashboard after submit
            return RedirectToAction("Index", "Dashboard", new { area = "Lecturer" });
        }
    }
}