using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Infrastructure.Persistence;

internal static class DatabaseExtensions
{
    /// <summary>
    /// Applies pending migrations (and runs seeding) at startup. Convenient for
    /// development and the demo; a real production system would run migrations
    /// as a separate deployment step.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FerryDbContext>();
        await context.Database.MigrateAsync();
    }
}
