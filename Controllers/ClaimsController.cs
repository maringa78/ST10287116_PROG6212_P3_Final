using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10287116_PROG6212_POE_P2.Services;
using ST10287116_PROG6212_POE_P2.Models;
using Microsoft.AspNetCore.Http;
using ST10287116_PROG6212_POE_P2.Services.Validation;
using ST10287116_PROG6212_POE_P2.Data;
using System.Security.Claims; 

namespace ST10287116_PROG6212_POE_P2.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class ClaimsController : Controller
    {
        private readonly ClaimService _service;
        private readonly ILogger<ClaimsController> _log;
        private readonly ApplicationDbContext _claimServiceContext; // Dependency for context
        public ClaimsController(ClaimService service, ILogger<ClaimsController> log, ApplicationDbContext claimServiceContext)
        {
            _service = service;
            _log = log;
            _claimServiceContext = claimServiceContext; // Initialize context
        }

        public IActionResult Submit() => View(new ST10287116_PROG6212_POE_P2.Models.Claim()); // Claim entity

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ST10287116_PROG6212_POE_P2.Models.Claim model, List<IFormFile> files)  // Files param for uploads
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // Return with errors (e.g, invalid rate)
            }

            model.ClaimDate = model.ClaimDate == default ? DateTime.Now : model.ClaimDate;
            model.Status = ClaimStatus.Pending;  // Initial status for workflow (coordinator verifies next)
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            model.UserId = userId;

            // Fetch lecturer for rate
            var lecturer = _claimServiceContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);
          
            int lecturerIntId = lecturer?.Id ?? 0;
            var monthHours = _service.GetMonthlyHoursForUser(lecturerIntId, DateTime.Now.Year, DateTime.Now.Month);
            if (monthHours + model.HoursWorked > 180)
            {
                ModelState.AddModelError("", "Monthly hour limit (180) exceeded.");
                return View(model);
            }

            model.LecturerId = 1;  // Ensure lecturer filter
            model.Created = DateTime.Now;
            model.LastUpdated = DateTime.Now;
            model.ClaimMonth = model.ClaimDate.ToString("MMMM yyyy");  // NEW: Auto-set e.g., "November 2025"

            //  Calculate Total Amount (Hours * Rate, fallback to Amount)
            if (model.HoursWorked > 0 && model.HourlyRate > 0)
            {
                model.TotalAmount = model.HoursWorked * model.HourlyRate;
            }
            else
            {
                model.TotalAmount = model.Amount;
            }

            //  Handle supporting documents upload
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Validate size <5MB and type (PDF or image)
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File too large (max 5MB).");
                        return View(model);
                    }
                    if (file.ContentType != "application/pdf" && !file.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("", "Invalid file type (PDF or image only).");
                        return View(model);
                    }

                    // Save file with unique name
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Adds toDocuments list for viewing in admin
                    model.Documents.Add(new Document
                    {
                        FileName = file.FileName,
                        FilePath = $"/uploads/{uniqueFileName}"
                    });
                }
            }

            // after binding model and before saving:
            var uid = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(uid)) return RedirectToAction("Login", "Account");
            model.UserId = uid;
            var lecturerEntity = _claimServiceContext.Users.FirstOrDefault(u => u.Id.ToString() == uid);
            var monthlyHours = _service.GetMonthlyHoursForUser(lecturerEntity!.Id, DateTime.Now.Year, DateTime.Now.Month);
            var (ok, error) = ClaimValidator.ValidateBusiness(model, monthlyHours);
            if (!ok)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            _service.Create(model);  // Save claim + documents

            return RedirectToAction("Index", "Dashboard", new { area = "Lecturer" });
        }

        public IActionResult Status()
        {
            // insures is not null before calling GetUserClaims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to login or handle as appropriate
                return RedirectToAction("Login", "Account");
            }

            var mine = _service.GetUserClaims(userId);

            return View(mine);
        }
    }
}