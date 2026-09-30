using System.Net;
using System.Net.Http.Json;
using FerryBooking.Api.Features.Sailings;
using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FerryBooking.IntegrationTests;

public class SailingsTests(FerryApiFactory factory)
{
    private static readonly TimeZoneInfo Athens = TimeZoneInfo.FindSystemTimeZoneById("Europe/Athens");

    private readonly HttpClient _client = factory.CreateClient();

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    private static DateOnly TomorrowInGreece =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Athens).DateTime).AddDays(1);

    [Fact]
    public async Task Search_ReturnsSailingsOnRouteDepartingThatDayInGreekTime()
    {
        var date = TomorrowInGreece;

        var sailings = await _client.GetFromJsonAsync<SailingResponse[]>(
            $"/api/sailings?from=pir&to=SDA&date={date:yyyy-MM-dd}", CancellationToken);

        Assert.NotNull(sailings);
        Assert.NotEmpty(sailings);
        Assert.All(sailings, s =>
        {
            Assert.Equal("PIR", s.Origin.Code);
            Assert.Equal("SDA", s.Destination.Code);
            Assert.Equal(date, DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(s.DepartureTime, Athens).DateTime));
        });
    }

    [Fact]
    public async Task Search_WithInvalidParameters_ReturnsValidationProblem()
    {
        var response = await _client.GetAsync(
            $"/api/sailings?from=PIRAEUS&to=SDA&date={TomorrowInGreece:yyyy-MM-dd}&passengers=50", CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync(CancellationToken);
        Assert.Contains("From", body, StringComparison.Ordinal);
        Assert.Contains("Passengers", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Schedule_ThenGet_ReturnsTheNewSailing()
    {
        var request = await ValidRequestAsync();

        var response = await _client.PostAsJsonAsync("/api/sailings", request, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<SailingResponse>(CancellationToken);
        Assert.NotNull(created);

        var fetched = await _client.GetFromJsonAsync<SailingResponse>(response.Headers.Location, CancellationToken);
        Assert.Equal(created, fetched);
        Assert.Equal(request.DepartureTime, fetched!.DepartureTime);
    }

    [Fact]
    public async Task Schedule_InThePast_ReturnsUnprocessableEntity()
    {
        var request = await ValidRequestAsync() with
        {
            DepartureTime = DateTimeOffset.UtcNow.AddDays(-1),
            ArrivalTime = DateTimeOffset.UtcNow.AddDays(-1).AddHours(9),
        };

        var response = await _client.PostAsJsonAsync("/api/sailings", request, CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Schedule_WithUnknownPort_ReturnsValidationProblem()
    {
        var request = await ValidRequestAsync() with { DestinationPortCode = "XXX" };

        var response = await _client.PostAsJsonAsync("/api/sailings", request, CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("DestinationPortCode", await response.Content.ReadAsStringAsync(CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Get_UnknownSailing_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/sailings/{Guid.NewGuid()}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ScheduleSailingRequest> ValidRequestAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FerryDbContext>();
        var vesselId = await db.Vessels.Select(v => v.Id).FirstAsync(CancellationToken);

        var departure = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(60).AddHours(18), TimeSpan.Zero);
        return new ScheduleSailingRequest("PIR", "HER", vesselId, departure, departure.AddHours(9), 45.00m);
    }
}
