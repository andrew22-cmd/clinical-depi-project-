using clinicsystem.Models;
using Microsoft.AspNetCore.Identity;

namespace clinicsystem.Data
{
    public static class UserRoles
    {
        public const string Manager = "Manager";
        public const string Doctor = "Doctor";
        public const string Secretary = "Secretary";
        public const string Patient = "Patient";
    }

    public class AppDbInitializer
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            // ── 1. Roles ─────────────────────────────────────────────────────
            var roleManager = scope.ServiceProvider
                                   .GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[]
            {
                UserRoles.Manager,
                UserRoles.Doctor,
                UserRoles.Secretary,
                UserRoles.Patient
            })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── 2. Default accounts ──────────────────────────────────────────
            var userManager = scope.ServiceProvider
                                   .GetRequiredService<UserManager<User>>();

            await EnsureUser(userManager,
                firstName: "Clinic", lastName: "Manager",
                email: "manager@clinic.com",
                role: UserRoles.Manager,
                password: "Manager*123");

            await EnsureUser(userManager,
                firstName: "Clinic", lastName: "Doctor",
                email: "doctor@clinic.com",
                role: UserRoles.Doctor,
                password: "Doctor*123");

            await EnsureUser(userManager,
                firstName: "Clinic", lastName: "Secretary",
                email: "secretary@clinic.com",
                role: UserRoles.Secretary,
                password: "Secretary*123");

            await EnsureUser(userManager,
                firstName: "Clinic", lastName: "Patient",
                email: "patient@clinic.com",
                role: UserRoles.Patient,
                password: "Patient*123");
        }

        // ── Private helper ────────────────────────────────────────────────────
        private static async Task EnsureUser(
            UserManager<User> userManager,
            string firstName, string lastName,
            string email, string role, string password)
        {
            // Skip if already seeded
            if (await userManager.FindByEmailAsync(email) != null)
                return;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = email,   // Identity uses UserName for sign-in
                Email = email,
                EmailConfirmed = true,    // no email verification in this project
                Role = role
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}