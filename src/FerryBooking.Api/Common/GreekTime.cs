namespace FerryBooking.Api.Common;

/// <summary>
/// Ferry timetables are published in Greek local time, so "sailings on 1 October"
/// means 1 October in Athens, not in UTC.
/// </summary>
internal static class GreekTime
{
    public static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Athens");

    /// <summary>Returns the UTC instants covering the given calendar day in Greece.</summary>
    public static (DateTimeOffset StartUtc, DateTimeOffset EndUtc) DayRangeUtc(DateOnly date)
    {
        return (ToUtc(date), ToUtc(date.AddDays(1)));

        static DateTimeOffset ToUtc(DateOnly day)
        {
            var localMidnight = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
            return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(localMidnight, Zone), TimeSpan.Zero);
        }
    }

    /// <summary>Creates an instant from a Greek local date and time.</summary>
    public static DateTimeOffset At(DateOnly date, TimeOnly time)
    {
        var local = date.ToDateTime(time, DateTimeKind.Unspecified);
        return new DateTimeOffset(local, Zone.GetUtcOffset(local));
    }
}
