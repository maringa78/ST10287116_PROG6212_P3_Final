using Microsoft.EntityFrameworkCore;
using ST10287116_PROG6212_POE_P2.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims { get; set; }

public DbSet<ST10287116_PROG6212_POE_P2.Models.User> User { get; set; } = default!;
    // Replace `object? Users` with the correct DbSet<TUser> if you have a user entity:
    // public DbSet<ApplicationUser> Users { get; set; }
}