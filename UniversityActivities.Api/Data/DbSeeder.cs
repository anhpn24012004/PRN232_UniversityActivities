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
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedActivitiesAsync(dbContext, userManager);
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

        private static async Task SeedActivitiesAsync(
            ApplicationDbContext dbContext,
            UserManager<AppUser> userManager)
        {
            if (dbContext.Activities.Any())
            {
                return;
            }

            var organizer = await userManager.FindByEmailAsync("organizer@university.com");

            if (organizer == null)
            {
                return;
            }

            var activities = new List<Activity>
            {
                new Activity
                {
                    Title = "AI Workshop for Students",
                    Description = "Introduction to Artificial Intelligence and practical AI tools.",
                    Location = "Room A101",
                    StartTime = DateTime.Now.AddDays(10),
                    EndTime = DateTime.Now.AddDays(10).AddHours(3),
                    MaxParticipants = 50,
                    Type = ActivityType.Workshop,
                    Status = ActivityStatus.Approved,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Web Development Workshop",
                    Description = "Basic ASP.NET Core Web API and frontend integration.",
                    Location = "Lab B202",
                    StartTime = DateTime.Now.AddDays(15),
                    EndTime = DateTime.Now.AddDays(15).AddHours(3),
                    MaxParticipants = 40,
                    Type = ActivityType.Workshop,
                    Status = ActivityStatus.Pending,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Academic Research Seminar",
                    Description = "Seminar about academic research methods for students.",
                    Location = "Hall C",
                    StartTime = DateTime.Now.AddDays(20),
                    EndTime = DateTime.Now.AddDays(20).AddHours(2),
                    MaxParticipants = 100,
                    Type = ActivityType.Seminar,
                    Status = ActivityStatus.Approved,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Career Orientation Seminar",
                    Description = "Career guidance and industry sharing session.",
                    Location = "Auditorium",
                    StartTime = DateTime.Now.AddDays(7),
                    EndTime = DateTime.Now.AddDays(7).AddHours(2),
                    MaxParticipants = 120,
                    Type = ActivityType.Seminar,
                    Status = ActivityStatus.Rejected,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Programming Competition",
                    Description = "Competitive programming contest for IT students.",
                    Location = "Lab D303",
                    StartTime = DateTime.Now.AddDays(25),
                    EndTime = DateTime.Now.AddDays(25).AddHours(4),
                    MaxParticipants = 60,
                    Type = ActivityType.Competition,
                    Status = ActivityStatus.Approved,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "English Debate Competition",
                    Description = "English debate contest for university students.",
                    Location = "Room E404",
                    StartTime = DateTime.Now.AddDays(30),
                    EndTime = DateTime.Now.AddDays(30).AddHours(3),
                    MaxParticipants = 30,
                    Type = ActivityType.Competition,
                    Status = ActivityStatus.Pending,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Green Sunday Volunteer Activity",
                    Description = "Community cleaning and environmental protection activity.",
                    Location = "City Park",
                    StartTime = DateTime.Now.AddDays(12),
                    EndTime = DateTime.Now.AddDays(12).AddHours(5),
                    MaxParticipants = 80,
                    Type = ActivityType.Volunteer,
                    Status = ActivityStatus.Approved,
                    OrganizerId = organizer.Id
                },

                new Activity
                {
                    Title = "Music Club Welcome Event",
                    Description = "Welcome event for new members of the university music club.",
                    Location = "Student Center",
                    StartTime = DateTime.Now.AddDays(-10),
                    EndTime = DateTime.Now.AddDays(-10).AddHours(3),
                    MaxParticipants = 70,
                    Type = ActivityType.ClubEvent,
                    Status = ActivityStatus.Approved,
                    OrganizerId = organizer.Id
                }
            };

            await dbContext.Activities.AddRangeAsync(activities);
            await dbContext.SaveChangesAsync();
        }
    }
}