using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Services;

namespace ST10287116_PROG6212_POE_P2.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class WorkflowController : Controller
    {
        private readonly ClaimService _claimService;
        public WorkflowController(ClaimService claimService) => _claimService = claimService;

        public IActionResult Verified() =>
            View(_claimService.GetVerifiedClaims());

        [HttpPost]
        public IActionResult Approve(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Approved);
            return RedirectToAction("Verified");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            _claimService.UpdateStatus(id, ClaimStatus.Rejected);
            return RedirectToAction("Verified");
        }
    }
}