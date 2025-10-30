using Microsoft.EntityFrameworkCore;
using PersonalReminderTool.Api.Data;
using PersonalReminderTool.Api.Data.Entities;

namespace PersonalReminderTool.Api.Extensions;

internal static class DatabaseExtensions
{
    public static async Task SetupPostgreSQLDatabaseAsync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(async options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.SeedDataAsync(configuration);
        });
    }

    public static async Task MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    private static void SeedDataAsync(this DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var baseUser = configuration
            .GetSection("BaseUser")
            .Get<User>()!;

        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            bool exists = await context.Set<User>().AnyAsync(u => u.Id == baseUser.Id, cancellationToken);

            if (!exists)
            {
                await context.Set<User>().AddAsync(baseUser);
                await context.SaveChangesAsync();
            }
        });

        options.UseSeeding((context, _) =>
        {
            bool exists = context.Set<User>().Any(u => u.Id == baseUser.Id);

            if (!exists)
            {
                context.Set<User>().Add(baseUser);
                context.SaveChanges();
            }
        });
    }
}
