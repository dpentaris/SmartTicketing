namespace FerryBooking.Api.Domain;

public sealed class Port
{
    private Port()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Short, unique code such as "PIR" for Piraeus.</summary>
    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public static Port Create(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
        {
            throw new DomainException("Port code must be exactly 3 characters.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Port name is required.");
        }

        return new Port
        {
            Id = Guid.CreateVersion7(),
            Code = code.ToUpperInvariant(),
            Name = name.Trim(),
        };
    }
}
