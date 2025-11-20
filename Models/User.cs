using System.ComponentModel.DataAnnotations;
namespace ST10287116_PROG6212_POE_P2.Models
{
    public enum UserRole { Lecturer, Coordinator, Manager, HR }

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty; // Keep if you still use it for display

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Plain for now

        public UserRole Role { get; set; }

        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; } = 25.00m;
    }
}
