using FerryBooking.Api.Common;
using FerryBooking.Api.Features.Ports;
using FerryBooking.Api.Features.Sailings;
using FerryBooking.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// The "ferrydb" connection string is injected by the Aspire AppHost
// (or set directly in configuration when running without Aspire).
builder.AddNpgsqlDbContext<FerryDbContext>("ferrydb", configureDbContextOptions: options => options
    .UseSeeding((context, _) => SeedData.Seed(context))
    .UseAsyncSeeding((context, _, cancellationToken) => SeedData.SeedAsync(context, cancellationToken)));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddValidation();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await app.MigrateDatabaseAsync();
}

app.MapDefaultEndpoints();
app.MapPortEndpoints();
app.MapSailingEndpoints();

await app.RunAsync();
