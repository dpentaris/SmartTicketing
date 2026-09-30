using System.Linq.Expressions;
using FerryBooking.Api.Domain;
using FerryBooking.Api.Features.Ports;

namespace FerryBooking.Api.Features.Sailings;

public sealed record SailingResponse(
    Guid Id,
    PortResponse Origin,
    PortResponse Destination,
    string Vessel,
    DateTimeOffset DepartureTime,
    DateTimeOffset ArrivalTime,
    decimal FarePerPassenger,
    int SeatsAvailable)
{
    /// <summary>
    /// Projection EF Core translates to SQL, so only the needed columns are loaded.
    /// </summary>
    public static readonly Expression<Func<Sailing, SailingResponse>> Projection = s => new SailingResponse(
        s.Id,
        new PortResponse(s.Origin.Code, s.Origin.Name),
        new PortResponse(s.Destination.Code, s.Destination.Name),
        s.Vessel.Name,
        s.DepartureTime,
        s.ArrivalTime,
        s.FarePerPassenger,
        s.Capacity - s.SeatsReserved);
}
