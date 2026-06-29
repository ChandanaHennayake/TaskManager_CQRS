using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Security;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.DBSeed;

public static class UserSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Username = "admin",
            PasswordHash = passwordHasher.HashPassword("Admin@123")
        };

        await context.Users.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}