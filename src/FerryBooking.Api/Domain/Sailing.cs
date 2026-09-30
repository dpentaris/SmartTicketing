namespace FerryBooking.Api.Domain;

/// <summary>
/// A single scheduled trip of a vessel from one port to another.
/// </summary>
public sealed class Sailing
{
    private Sailing()
    {
    }

    public Guid Id { get; private set; }

    public Guid OriginPortId { get; private set; }

    public Port Origin { get; private set; } = null!;

    public Guid DestinationPortId { get; private set; }

    public Port Destination { get; private set; } = null!;

    public Guid VesselId { get; private set; }

    public Vessel Vessel { get; private set; } = null!;

    public DateTimeOffset DepartureTime { get; private set; }

    public DateTimeOffset ArrivalTime { get; private set; }

    /// <summary>Price of one passenger ticket, in euros.</summary>
    public decimal FarePerPassenger { get; private set; }

    /// <summary>Seats offered on this sailing, copied from the vessel when scheduled.</summary>
    public int Capacity { get; private set; }

    public int SeatsReserved { get; private set; }

    public int SeatsAvailable => Capacity - SeatsReserved;

    public TimeSpan Duration => ArrivalTime - DepartureTime;

    public static Sailing Schedule(
        Port origin,
        Port destination,
        Vessel vessel,
        DateTimeOffset departureTime,
        DateTimeOffset arrivalTime,
        decimal farePerPassenger,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(vessel);

        if (origin.Id == destination.Id)
        {
            throw new DomainException("Origin and destination must be different ports.");
        }

        if (departureTime <= now)
        {
            throw new DomainException("Departure time must be in the future.");
        }

        if (arrivalTime <= departureTime)
        {
            throw new DomainException("Arrival time must be after departure time.");
        }

        if (farePerPassenger <= 0)
        {
            throw new DomainException("Fare must be positive.");
        }

        return new Sailing
        {
            Id = Guid.CreateVersion7(),
            Origin = origin,
            OriginPortId = origin.Id,
            Destination = destination,
            DestinationPortId = destination.Id,
            Vessel = vessel,
            VesselId = vessel.Id,
            // PostgreSQL "timestamp with time zone" columns only accept UTC values.
            DepartureTime = departureTime.ToUniversalTime(),
            ArrivalTime = arrivalTime.ToUniversalTime(),
            FarePerPassenger = farePerPassenger,
            Capacity = vessel.PassengerCapacity,
        };
    }
}
