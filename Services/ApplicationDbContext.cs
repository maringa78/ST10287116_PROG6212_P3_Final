using Microsoft.EntityFrameworkCore;
using ST10287116_PROG6212_POE_P2.Models;  // For Claim, User, Document

namespace ST10287116_PROG6212_POE_P2.Data  // Public namespace
{
    public class ApplicationDbContext : DbContext  // Now PUBLIC class
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Fixes CS1061: Add DbSets here
        public DbSet<User> Users { get; set; } = null!;    // For AuthService
        public DbSet<Claim> Claims { get; set; } = null!;  // For _context.Claims
        public DbSet<Document> Documents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure entities (e.g., ClaimId as key)
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClaimId).IsRequired();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}