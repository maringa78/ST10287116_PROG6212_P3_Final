using ST10287116_PROG6212_POE_P2.Models;
using ST10287116_PROG6212_POE_P2.Data;
using Microsoft.EntityFrameworkCore;

namespace ST10287116_PROG6212_POE_P2.Services
{
    public class ClaimService
    {
        private readonly ApplicationDbContext _context;
        public ClaimService(ApplicationDbContext context) => _context = context;

        public int GetMonthlyHoursForUser(int userId, int year, int month)
        {
            return _context.Claims
                .Where(c => c.LecturerId == userId &&
                            c.ClaimDate.Year == year &&
                            c.ClaimDate.Month == month)
                .Sum(c => c.HoursWorked);
        }

        public IEnumerable<Claim> GetUserClaims(string userId) =>
            _context.Claims.Where(c => c.UserId == userId).ToList();

        public IEnumerable<Claim> GetAllClaims(string? search = "")
        {
            var query = _context.Claims.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ClaimId.Contains(search) || c.Status.ToString().Contains(search));
            }
            return query.ToList();
        }

        public IEnumerable<Claim> GetPendingClaims(string? search = "")
        {
            var query = _context.Claims
                .Where(c => c.Status == ClaimStatus.Pending)
                .Include(c => c.Documents)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ClaimId.Contains(search));
            }
            return query.ToList();
        }

        public IEnumerable<Claim> GetPendingClaims() =>
            _context.Claims
                .Where(c => c.Status == ClaimStatus.Pending)
                .Include(c => c.Documents)
                .ToList();

        public IEnumerable<Claim> GetVerifiedClaims() =>
            _context.Claims
                .Where(c => c.Status == ClaimStatus.Verified)
                .Include(c => c.Documents)
                .ToList();

        public void UpdateStatus(int id, ClaimStatus status)
        {
            var claim = _context.Claims.Find(id);
            if (claim != null)
            {
                claim.Status = status;
                claim.LastUpdated = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void CreateClaim(Claim claim)
        {
            if (!string.IsNullOrWhiteSpace(claim.UserId))
            {
                var lecturer = _context.User.FirstOrDefault(u => u.Id.ToString() == claim.UserId);
                if (lecturer != null)
                {
                    claim.HourlyRate = lecturer.HourlyRate;
                    claim.LecturerId = lecturer.Id;
                }
            }
            claim.TotalAmount = claim.HourlyRate * claim.HoursWorked;
            claim.Created = DateTime.Now;
            claim.LastUpdated = DateTime.Now;

            _context.Claims.Add(claim);
            if (claim.Documents?.Count > 0)
            {
                _context.Set<Document>().AddRange(claim.Documents);
            }
            _context.SaveChanges();
        }

        // HR reports: approved claims summary (CSV-friendly data)
        public IEnumerable<(string ClaimId, string LecturerEmail, int Hours, decimal Rate, decimal Total, DateTime Date)>
            GetApprovedClaimsReport(DateTime? from = null, DateTime? to = null)
        {
            var q = _context.Claims
                .Include(c => c.Documents)
                .Where(c => c.Status == ClaimStatus.Approved);

            if (from.HasValue) q = q.Where(c => c.ClaimDate >= from.Value);
            if (to.HasValue) q = q.Where(c => c.ClaimDate <= to.Value);

            return q.Select(c => new
            {
                c.ClaimId,
                LecturerEmail = _context.Set<User>().Where(u => u.Id.ToString() == c.UserId).Select(u => u.Email).FirstOrDefault(),
                Hours = c.HoursWorked,
                Rate = c.HourlyRate,
                Total = c.TotalAmount,
                Date = c.ClaimDate
            })
            .AsEnumerable()
            .Select(x => (x.ClaimId, x.LecturerEmail ?? string.Empty, x.Hours, x.Rate, x.Total, x.Date));
        }
    }
}