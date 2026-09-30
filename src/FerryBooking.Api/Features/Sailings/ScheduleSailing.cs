using System.ComponentModel.DataAnnotations;
using FerryBooking.Api.Domain;
using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Features.Sailings;

public sealed record ScheduleSailingRequest(
    [property: Required, StringLength(3, MinimumLength = 3)] string OriginPortCode,
    [property: Required, StringLength(3, MinimumLength = 3)] string DestinationPortCode,
    Guid VesselId,
    DateTimeOffset DepartureTime,
    DateTimeOffset ArrivalTime,
    [property: Range(0.01, 10_000)] decimal FarePerPassenger);

internal static class ScheduleSailing
{
    public static async Task<Results<Created<SailingResponse>, ValidationProblem>> Handle(
        ScheduleSailingRequest request,
        FerryDbContext db,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var codes = new[] { request.OriginPortCode.ToUpperInvariant(), request.DestinationPortCode.ToUpperInvariant() };
        var ports = await db.Ports
            .Where(p => codes.Contains(p.Code))
            .ToDictionaryAsync(p => p.Code, cancellationToken);
        var vessel = await db.Vessels.FindAsync([request.VesselId], cancellationToken);

        var errors = new Dictionary<string, string[]>();
        if (!ports.TryGetValue(codes[0], out var origin))
        {
            errors[nameof(request.OriginPortCode)] = [$"Unknown port '{request.OriginPortCode}'."];
        }

        if (!ports.TryGetValue(codes[1], out var destination))
        {
            errors[nameof(request.DestinationPortCode)] = [$"Unknown port '{request.DestinationPortCode}'."];
        }

        if (vessel is null)
        {
            errors[nameof(request.VesselId)] = ["Unknown vessel."];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        // Business rules (future departure, arrival after departure, ...) live in the
        // domain; a violation throws DomainException and becomes a 422 response.
        var sailing = Sailing.Schedule(
            origin!,
            destination!,
            vessel!,
            request.DepartureTime,
            request.ArrivalTime,
            request.FarePerPassenger,
            timeProvider.GetUtcNow());

        db.Sailings.Add(sailing);
        await db.SaveChangesAsync(cancellationToken);

        var response = SailingResponse.Projection.Compile()(sailing);
        return TypedResults.Created($"/api/sailings/{sailing.Id}", response);
    }
}
