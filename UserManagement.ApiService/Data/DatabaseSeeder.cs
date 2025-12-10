using UserManagement.ApiService.Models;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(Context context)
    {
        // لو فيه رولز قبل كده متضيفش تاني
        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role
            {
                ID = Guid.NewGuid(),
                Name = "Admin",
                Description = "Administrator Role",
                CreatedBy = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            };

            var userRole = new Role
            {
                ID = Guid.NewGuid(),
                Name = "User",
                Description = "Standard User Role",
                CreatedBy = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            };

            await context.Roles.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();

            // Create Admin User
            var passwordHasher = new PasswordHasher<string>();
            var hashedPassword = passwordHasher.HashPassword(null, "Admin123");

            var adminUser = new User
            {
                ID = Guid.NewGuid(),
                Email = "upskillingfinalproject@gmail.com",
                Password = hashedPassword,
                Name = "Admin User",
                PhoneNo = "1234567890",
                Country = "CountryName",
                IsActive = true,
                TwoFactorAuthEnabled = false,
                IsEmailConfirmed = true,
                RoleID = adminRole.ID,
                CreatedBy = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Add all features for Admin
            var features = Enum.GetValues(typeof(Feature)).Cast<Feature>().ToList();
            var userFeatures = features.Select(f => new UserFeature
            {
                ID = Guid.NewGuid(),
                UserID = adminUser.ID,
                Feature = f,
                CreatedBy = Guid.Empty,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await context.UserFeatures.AddRangeAsync(userFeatures);
            await context.SaveChangesAsync();
        }
    }
}
