using ST10287116_PROG6212_POE_P2.Data;    // For ApplicationDbContext
using ST10287116_PROG6212_POE_P2.Models;  // For User – fixes CS0246
using Microsoft.EntityFrameworkCore;       // Add this for EF Core LINQ support
using System.Linq;                        // Add this for LINQ extension methods

namespace ST10287116_PROG6212_POE_P2.Services  // Single definition here
{
    public class AuthService  // No duplicates now

    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fix: Use Set<User>() to get the correct DbSet<User>
        public User? ValidateUser(string email, string password)
        {
            // Use proper password hashing in real apps
            return _context.Set<User>().FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
        }
    }
}