using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions.Security;
using TaskManager.Infrastructure.Persistence.DBSeed;

namespace TaskManager.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        await UserSeeder.SeedAsync(
            context,
            passwordHasher);

        await TaskSeeder.SeedAsync(context);
    }
}