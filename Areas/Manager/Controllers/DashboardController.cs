using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Services;

namespace ST10287116_PROG6212_POE_P2.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DashboardController : Controller
    {
        private readonly ClaimService _claimService;
        public DashboardController(ClaimService claimService)
        {
            _claimService = claimService;
        }
        public IActionResult Index(string search = "")
        {
            var claims = _claimService.GetPendingClaims(search);
            return View(claims);
        }
        [HttpPost]
        public IActionResult Approve(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Approved);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Reject(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Rejected);
            return RedirectToAction("Index");
        }
    }
}
