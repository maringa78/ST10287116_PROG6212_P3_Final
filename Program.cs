using System;
using Microsoft.EntityFrameworkCore;
using ST10287116_PROG6212_POE_P2.Services;

namespace ST10287116_PROG6212_POE_P2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Register MVC controllers with views (so controllers like AccountController are routable)
            builder.Services.AddControllersWithViews();

            // Register session support used by AccountController
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register application services (adjust lifetime/type as appropriate)
            builder.Services.AddScoped<Services.AuthService>();
            builder.Services.AddScoped<ClaimService>(); // <-- register ClaimService

            // Use an in-memory EF provider (no connection string required).
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("AppInMemoryDb"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "root-to-lecturer",
                pattern: "",
                defaults: new { area = "Lecturer", controller = "Dashboard", action = "Index" });

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
