using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<ActivityRegistration> ActivityRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Activity>()
                .HasOne(a => a.Organizer)
                .WithMany()
                .HasForeignKey(a => a.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ActivityRegistration>()
                .HasOne(r => r.Activity)
                .WithMany(a => a.Registrations)
                .HasForeignKey(r => r.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ActivityRegistration>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ActivityRegistration>()
                .HasIndex(r => new { r.ActivityId, r.StudentId })
                .IsUnique();
        }
    }
}

