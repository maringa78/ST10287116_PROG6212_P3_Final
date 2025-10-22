using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
namespace ST10287116_PROG6212_POE_P2.Models
{
    public enum ClaimStatus { Pending, Approved, Rejected }
    public enum ClaimType { Travel, Equipment, Research, Conference, Development }
    public class Claim
    {
        public int Id { get; set; }
        [Required]
        public string ClaimId { get; set; } = "CLM-" + DateTime.Now.ToString("yyyy-MM-dd");
        [Required]
        public DateTime ClaimDate { get; set; }
        [Required]
        public ClaimType Type { get; set; }
        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public int? HoursWorked { get; set; }
        public string? ClaimMonth { get; set; }  // e.g., "June 2025"
        public string? UserId { get; set; }
        public List<Document>? Documents { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int LecturerId { get; internal set; }
        public DateTime Created { get; internal set; }
    }
    public class Document
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public int ClaimId { get; set; }
    }
}
