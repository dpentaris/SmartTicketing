namespace FerryBooking.Api.Domain;

public sealed class Vessel
{
    private Vessel()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public int PassengerCapacity { get; private set; }

    public static Vessel Create(string name, int passengerCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Vessel name is required.");
        }

        if (passengerCapacity <= 0)
        {
            throw new DomainException("Passenger capacity must be positive.");
        }

        return new Vessel
        {
            Id = Guid.CreateVersion7(),
            Name = name.Trim(),
            PassengerCapacity = passengerCapacity,
        };
    }
}
