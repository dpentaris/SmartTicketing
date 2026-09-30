using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Features.Ports;

public sealed record PortResponse(string Code, string Name);

internal static class PortEndpoints
{
    public static IEndpointRouteBuilder MapPortEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/ports", GetPorts)
            .WithName("GetPorts")
            .WithTags("Ports")
            .WithSummary("Lists all ports served.");

        return app;
    }

    private static async Task<PortResponse[]> GetPorts(FerryDbContext db, CancellationToken cancellationToken) =>
        await db.Ports
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new PortResponse(p.Code, p.Name))
            .ToArrayAsync(cancellationToken);
}
