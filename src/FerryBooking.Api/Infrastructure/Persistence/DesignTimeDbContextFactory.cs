using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FerryBooking.Api.Infrastructure.Persistence;

/// <summary>
/// Used only by the <c>dotnet ef</c> tooling to create migrations, so it does not
/// need the Aspire-provided connection string that the running app gets.
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FerryDbContext>
{
    public FerryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FerryDbContext>()
            .UseNpgsql("Host=localhost;Database=ferrydb")
            .Options;

        return new FerryDbContext(options);
    }
}
