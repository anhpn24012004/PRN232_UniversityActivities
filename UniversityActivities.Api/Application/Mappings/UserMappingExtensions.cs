using UniversityActivities.Api.Application.DTOs.Users;
using UniversityActivities.Api.Domain.Entities;

namespace UniversityActivities.Api.Application.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserResponse ToUserResponse(this AppUser user, IList<string> roles)
        {
            return new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                StudentCode = user.StudentCode,
                Department = user.Department,
                Roles = roles,
                IsLocked = user.LockoutEnd.HasValue &&
                           user.LockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }
    }
}
