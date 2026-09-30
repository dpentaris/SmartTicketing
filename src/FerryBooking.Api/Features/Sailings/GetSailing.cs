using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Features.Sailings;

internal static class GetSailing
{
    public static async Task<Results<Ok<SailingResponse>, NotFound>> Handle(
        Guid id,
        FerryDbContext db,
        CancellationToken cancellationToken)
    {
        var sailing = await db.Sailings
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(SailingResponse.Projection)
            .SingleOrDefaultAsync(cancellationToken);

        return sailing is null ? TypedResults.NotFound() : TypedResults.Ok(sailing);
    }
}
