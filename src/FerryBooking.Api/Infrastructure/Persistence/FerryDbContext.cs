using FerryBooking.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FerryBooking.Api.Infrastructure.Persistence;

public sealed class FerryDbContext(DbContextOptions<FerryDbContext> options) : DbContext(options)
{
    public DbSet<Port> Ports => Set<Port>();

    public DbSet<Vessel> Vessels => Set<Vessel>();

    public DbSet<Sailing> Sailings => Set<Sailing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FerryDbContext).Assembly);
    }
}
