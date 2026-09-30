using FerryBooking.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerryBooking.Api.Infrastructure.Persistence.Configurations;

internal sealed class SailingConfiguration : IEntityTypeConfiguration<Sailing>
{
    public void Configure(EntityTypeBuilder<Sailing> builder)
    {
        builder.HasOne(s => s.Origin).WithMany().HasForeignKey(s => s.OriginPortId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Destination).WithMany().HasForeignKey(s => s.DestinationPortId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Vessel).WithMany().HasForeignKey(s => s.VesselId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.FarePerPassenger).HasPrecision(10, 2);

        builder.Ignore(s => s.SeatsAvailable);
        builder.Ignore(s => s.Duration);

        // Search queries filter by route and departure time.
        builder.HasIndex(s => new { s.OriginPortId, s.DestinationPortId, s.DepartureTime });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Sailings_DifferentPorts", "\"OriginPortId\" <> \"DestinationPortId\"");
            t.HasCheckConstraint("CK_Sailings_SeatsWithinCapacity", "\"SeatsReserved\" BETWEEN 0 AND \"Capacity\"");
        });
    }
}
