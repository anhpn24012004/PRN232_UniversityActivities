using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityActivities.Api.Data;
using UniversityActivities.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanDeleteActivity", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("OnlyStudent", policy =>
        policy.RequireRole("Student"));

    options.AddPolicy("OnlyStaffOrAdmin", policy =>
        policy.RequireRole("Staff", "Admin"));

    options.AddPolicy("OnlyOrganizer", policy =>
    policy.RequireRole("Organizer"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
