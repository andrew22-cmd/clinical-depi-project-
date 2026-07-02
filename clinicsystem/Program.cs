//using clinicsystem.Data;
//using clinicsystem.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace clinicsystem
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // ── MVC ──────────────────────────────────────────────────────────
//            builder.Services.AddControllersWithViews();

//            // ── Database ─────────────────────────────────────────────────────
//            builder.Services.AddDbContext<AppDbContext>(options =>
//                options.UseSqlServer(
//                    builder.Configuration.GetConnectionString("constring")));

//            // ── ASP.NET Core Identity ────────────────────────────────────────
//            builder.Services.AddIdentity<User, IdentityRole>(options =>
//            {
//                options.Password.RequireNonAlphanumeric = true;
//                options.Password.RequiredLength = 8;
//                options.Password.RequireUppercase = true;
//                options.Password.RequireLowercase = true;
//                options.Password.RequireDigit = true;

//                options.User.RequireUniqueEmail = true;

//                options.SignIn.RequireConfirmedAccount = true;
//                options.SignIn.RequireConfirmedEmail = true;
//            })
//            .AddEntityFrameworkStores<AppDbContext>()
//            .AddDefaultTokenProviders();

//            // ── Build ────────────────────────────────────────────────────────
//            var app = builder.Build();

//            // ── Middleware pipeline ──────────────────────────────────────────
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }

//            app.UseStaticFiles();
//            app.UseRouting();

//            // Order matters: Authentication before Authorization.
//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            // ── Seed roles + default accounts once at startup ────────────────
//            AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

//            app.Run();
//        }
//    }
//}
using clinicsystem.Data;
using clinicsystem.Models;
using clinicsystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace clinicsystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── MVC ──────────────────────────────────────────────────────────
            builder.Services.AddControllersWithViews();

            // ── Database ─────────────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("constring")));

            // ── ASP.NET Core Identity ────────────────────────────────────────
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
// Add before builder.Build()
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<Services.IEmailSender, Services.EmailService>();
            // ── Email (confirmation links, password resets) ─────────────────
            //builder.Services.Configure<EmailSettings>(
            //    builder.Configuration.GetSection("EmailSettings"));
            //builder.Services.AddScoped<Services.IEmailSender, EmailService>();

            // ── Build ────────────────────────────────────────────────────────
            var app = builder.Build();

            // ── Middleware pipeline ──────────────────────────────────────────
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            // Order matters: Authentication before Authorization.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ── Seed roles + default accounts once at startup ────────────────
            AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

            app.Run();
        }
    }
}