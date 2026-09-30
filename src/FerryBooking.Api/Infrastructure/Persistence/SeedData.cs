using FerryBooking.Api.Common;
using FerryBooking.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Infrastructure.Persistence;

/// <summary>
/// Demo timetable for local development and the live demo: a handful of Aegean
/// ports and two weeks of sailings starting tomorrow.
/// </summary>
internal static class SeedData
{
    private const int DaysToSchedule = 14;

    private sealed record Route(string From, string To, TimeOnly Departs, TimeSpan Duration, decimal Fare, int VesselIndex);

    private static readonly Route[] DailyRoutes =
    [
        new("PIR", "SDA", new TimeOnly(21, 0), TimeSpan.FromHours(9), 42.50m, 0),
        new("SDA", "PIR", new TimeOnly(21, 0), TimeSpan.FromHours(9), 42.50m, 0),
        new("PIR", "HER", new TimeOnly(21, 30), TimeSpan.FromHours(9), 44.00m, 1),
        new("HER", "PIR", new TimeOnly(21, 30), TimeSpan.FromHours(9), 44.00m, 1),
        new("PIR", "JTR", new TimeOnly(7, 25), TimeSpan.FromHours(5), 79.50m, 2),
        new("JTR", "PIR", new TimeOnly(15, 30), TimeSpan.FromHours(5), 79.50m, 2),
        new("HER", "JTR", new TimeOnly(9, 30), TimeSpan.FromHours(2), 69.00m, 3),
        new("JTR", "HER", new TimeOnly(17, 45), TimeSpan.FromHours(2), 69.00m, 3),
        new("PIR", "JMK", new TimeOnly(7, 0), TimeSpan.FromHours(5.5), 72.00m, 3),
    ];

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        if (await context.Set<Port>().AnyAsync(cancellationToken))
        {
            return;
        }

        AddTimetable(context);
        await context.SaveChangesAsync(cancellationToken);
    }

    public static void Seed(DbContext context)
    {
        if (context.Set<Port>().Any())
        {
            return;
        }

        AddTimetable(context);
        context.SaveChanges();
    }

    private static void AddTimetable(DbContext context)
    {
        var ports = new[]
        {
            Port.Create("PIR", "Piraeus"),
            Port.Create("SDA", "Chania (Souda)"),
            Port.Create("HER", "Heraklion"),
            Port.Create("JTR", "Santorini (Thira)"),
            Port.Create("JMK", "Mykonos"),
        }.ToDictionary(p => p.Code);

        Vessel[] vessels =
        [
            Vessel.Create("Kydon Palace", 1800),
            Vessel.Create("Knossos Star", 1600),
            Vessel.Create("Aegean Runner", 450),
            Vessel.Create("Cyclades Express", 800),
        ];

        context.AddRange(ports.Values);
        context.AddRange(vessels);

        var now = DateTimeOffset.UtcNow;
        var tomorrow = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, GreekTime.Zone).DateTime).AddDays(1);

        for (var day = 0; day < DaysToSchedule; day++)
        {
            var date = tomorrow.AddDays(day);
            foreach (var route in DailyRoutes)
            {
                var departure = GreekTime.At(date, route.Departs);
                context.Add(Sailing.Schedule(
                    ports[route.From],
                    ports[route.To],
                    vessels[route.VesselIndex],
                    departure,
                    departure + route.Duration,
                    route.Fare,
                    now));
            }
        }
    }
}
