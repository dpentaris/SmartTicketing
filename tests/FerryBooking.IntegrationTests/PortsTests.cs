using System.Net.Http.Json;
using FerryBooking.Api.Features.Ports;

namespace FerryBooking.IntegrationTests;

public class PortsTests(FerryApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPorts_ReturnsSeededPortsOrderedByName()
    {
        var ports = await _client.GetFromJsonAsync<PortResponse[]>("/api/ports", TestContext.Current.CancellationToken);

        Assert.NotNull(ports);
        Assert.Contains(ports, p => p.Code == "PIR" && p.Name == "Piraeus");
        Assert.Equal(ports.OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => p.Code), ports.Select(p => p.Code));
    }
}
