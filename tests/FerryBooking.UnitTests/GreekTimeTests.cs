using FerryBooking.Api.Common;

namespace FerryBooking.UnitTests;

public class GreekTimeTests
{
    [Fact]
    public void DayRangeUtc_InSummerTime_IsOffsetByThreeHours()
    {
        var (start, end) = GreekTime.DayRangeUtc(new DateOnly(2026, 7, 15));

        Assert.Equal(new DateTimeOffset(2026, 7, 14, 21, 0, 0, TimeSpan.Zero), start);
        Assert.Equal(new DateTimeOffset(2026, 7, 15, 21, 0, 0, TimeSpan.Zero), end);
    }

    [Fact]
    public void DayRangeUtc_InWinterTime_IsOffsetByTwoHours()
    {
        var (start, _) = GreekTime.DayRangeUtc(new DateOnly(2026, 1, 15));

        Assert.Equal(new DateTimeOffset(2026, 1, 14, 22, 0, 0, TimeSpan.Zero), start);
    }

    [Fact]
    public void DayRangeUtc_OnDaylightSavingChangeDay_Is23HoursLong()
    {
        // Clocks go forward in Greece on the last Sunday of March.
        var (start, end) = GreekTime.DayRangeUtc(new DateOnly(2026, 3, 29));

        Assert.Equal(TimeSpan.FromHours(23), end - start);
    }
}
