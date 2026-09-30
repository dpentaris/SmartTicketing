namespace FerryBooking.Api.Features.Sailings;

internal static class SailingEndpoints
{
    public static IEndpointRouteBuilder MapSailingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sailings").WithTags("Sailings");

        group.MapGet("/", SearchSailings.Handle)
            .WithName("SearchSailings")
            .WithSummary("Finds sailings on a route for a given day (Greek local time) with enough free seats.");

        group.MapGet("/{id:guid}", GetSailing.Handle)
            .WithName("GetSailing")
            .WithSummary("Gets a single sailing.");

        // TODO(week 2): restrict to the Admin role once authentication is in place.
        group.MapPost("/", ScheduleSailing.Handle)
            .WithName("ScheduleSailing")
            .WithSummary("Schedules a new sailing.");

        return app;
    }
}
