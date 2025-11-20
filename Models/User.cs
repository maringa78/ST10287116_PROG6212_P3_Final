using System.ComponentModel.DataAnnotations;
namespace ST10287116_PROG6212_POE_P2.Models
{
    public enum UserRole { Lecturer, Coordinator, Manager }
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;  // Hash in service
        public UserRole Role { get; set; }
        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; } = 25.00m;
    }
}
