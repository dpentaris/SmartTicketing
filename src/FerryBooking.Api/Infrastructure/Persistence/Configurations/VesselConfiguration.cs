using FerryBooking.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerryBooking.Api.Infrastructure.Persistence.Configurations;

internal sealed class VesselConfiguration : IEntityTypeConfiguration<Vessel>
{
    public void Configure(EntityTypeBuilder<Vessel> builder)
    {
        builder.Property(v => v.Name).HasMaxLength(100);
    }
}
