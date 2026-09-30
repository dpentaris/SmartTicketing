using FerryBooking.Api.Domain;

namespace FerryBooking.UnitTests;

public class SailingTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Port Piraeus = Port.Create("PIR", "Piraeus");
    private static readonly Port Chania = Port.Create("SDA", "Chania (Souda)");
    private static readonly Vessel Vessel = Vessel.Create("Kydon Palace", 1800);

    [Fact]
    public void Schedule_WithValidData_CopiesCapacityFromVesselAndHasAllSeatsFree()
    {
        var sailing = Sailing.Schedule(Piraeus, Chania, Vessel, Now.AddDays(1), Now.AddDays(1).AddHours(9), 42.50m, Now);

        Assert.Equal(1800, sailing.Capacity);
        Assert.Equal(1800, sailing.SeatsAvailable);
        Assert.Equal(TimeSpan.FromHours(9), sailing.Duration);
    }

    [Fact]
    public void Schedule_StoresTimesInUtc()
    {
        var departureInGreece = new DateTimeOffset(2026, 10, 2, 21, 0, 0, TimeSpan.FromHours(3));

        var sailing = Sailing.Schedule(Piraeus, Chania, Vessel, departureInGreece, departureInGreece.AddHours(9), 42.50m, Now);

        Assert.Equal(TimeSpan.Zero, sailing.DepartureTime.Offset);
        Assert.Equal(departureInGreece, sailing.DepartureTime);
    }

    [Fact]
    public void Schedule_ToSamePort_Throws()
    {
        var ex = Assert.Throws<DomainException>(() =>
            Sailing.Schedule(Piraeus, Piraeus, Vessel, Now.AddDays(1), Now.AddDays(1).AddHours(1), 10m, Now));

        Assert.Contains("different ports", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Schedule_InThePast_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Sailing.Schedule(Piraeus, Chania, Vessel, Now.AddMinutes(-1), Now.AddHours(9), 10m, Now));
    }

    [Fact]
    public void Schedule_ArrivingBeforeDeparture_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Sailing.Schedule(Piraeus, Chania, Vessel, Now.AddDays(1), Now.AddDays(1), 10m, Now));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Schedule_WithNonPositiveFare_Throws(decimal fare)
    {
        Assert.Throws<DomainException>(() =>
            Sailing.Schedule(Piraeus, Chania, Vessel, Now.AddDays(1), Now.AddDays(1).AddHours(9), fare, Now));
    }
}
