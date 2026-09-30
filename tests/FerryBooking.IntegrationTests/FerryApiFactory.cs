using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(FerryBooking.IntegrationTests.FerryApiFactory))]

namespace FerryBooking.IntegrationTests;

/// <summary>
/// Hosts the real API in memory against a throwaway PostgreSQL container, so tests
/// exercise the same SQL, migrations and constraints as production.
/// One container is shared by the whole test assembly.
/// </summary>
public sealed class FerryApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        // Start the host (which migrates and seeds) once, here, instead of lazily on the
        // first CreateClient() call: test classes run in parallel and would race to do it.
        StartServer();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:ferrydb", _database.GetConnectionString());
        builder.UseSetting("Database:MigrateOnStartup", "true");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();
    }
}
