using ST10287116_PROG6212_POE_P2.Models;

namespace ST10287116_PROG6212_POE_P2.Services.Validation
{
    public static class ClaimValidator
    {
        public static (bool ok, string? error) ValidateBusiness(Claim claim, int monthlyHoursSoFar)
        {
            if (claim.HoursWorked <= 0)
                return (false, "Hours must be greater than zero.");
            if (monthlyHoursSoFar + claim.HoursWorked > 180)
                return (false, "Monthly hour limit (180) exceeded.");
            if (claim.Documents.Count == 0)
                return (false, "At least one supporting document is required.");
            return (true, null);
        }
    }
}