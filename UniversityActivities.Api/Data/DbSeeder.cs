using Microsoft.AspNetCore.Identity;
using UniversityActivities.Api.Models;

namespace UniversityActivities.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                "Admin",
                "Staff",
                "Organizer",
                "Student"
            };

            foreach (var role in roles)
            {
                bool exists = await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            await CreateUserAsync(
                userManager,
                "admin@university.com",
                "Admin@123",
                "System Admin",
                "Administration",
                null,
                "Admin");

            await CreateUserAsync(
                userManager,
                "staff@university.com",
                "Staff@123",
                "Student Affairs Staff",
                "Student Affairs Office",
                null,
                "Staff");

            await CreateUserAsync(
                userManager,
                "organizer@university.com",
                "Organizer@123",
                "Default Organizer",
                "Information Technology",
                null,
                "Organizer");

            await CreateUserAsync(
                userManager,
                "student@university.com",
                "Student@123",
                "Default Student",
                "Information Technology",
                "SE000001",
                "Student");
        }

        private static async Task CreateUserAsync(
            UserManager<AppUser> userManager,
            string email,
            string password,
            string fullName,
            string department,
            string? studentCode,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return;
            }

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Department = department,
                StudentCode = studentCode,
                DateOfBirth = new DateTime(2003, 1, 1),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}