using System.ComponentModel.DataAnnotations;
using FerryBooking.Api.Common;
using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Features.Sailings;

public sealed record SearchSailingsRequest(
    [property: Required, StringLength(3, MinimumLength = 3)] string From,
    [property: Required, StringLength(3, MinimumLength = 3)] string To,
    DateOnly Date,
    [property: Range(1, 9)] int Passengers = 1);

internal static class SearchSailings
{
    public static async Task<SailingResponse[]> Handle(
        [AsParameters] SearchSailingsRequest request,
        FerryDbContext db,
        CancellationToken cancellationToken)
    {
        var from = request.From.ToUpperInvariant();
        var to = request.To.ToUpperInvariant();
        var (startUtc, endUtc) = GreekTime.DayRangeUtc(request.Date);

        return await db.Sailings
            .AsNoTracking()
            .Where(s => s.Origin.Code == from && s.Destination.Code == to)
            .Where(s => s.DepartureTime >= startUtc && s.DepartureTime < endUtc)
            .Where(s => s.Capacity - s.SeatsReserved >= request.Passengers)
            .OrderBy(s => s.DepartureTime)
            .Select(SailingResponse.Projection)
            .ToArrayAsync(cancellationToken);
    }
}
