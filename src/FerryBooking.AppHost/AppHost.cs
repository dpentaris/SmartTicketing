var builder = DistributedApplication.CreateBuilder(args);

// Aspire starts PostgreSQL in a Docker container for local development,
// so nothing needs to be installed except Docker itself.
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("ferrydb");

builder.AddProject<Projects.FerryBooking_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints();

builder.Build().Run();
