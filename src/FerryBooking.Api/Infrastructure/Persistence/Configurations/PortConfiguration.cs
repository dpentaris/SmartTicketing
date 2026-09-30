using FerryBooking.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerryBooking.Api.Infrastructure.Persistence.Configurations;

internal sealed class PortConfiguration : IEntityTypeConfiguration<Port>
{
    public void Configure(EntityTypeBuilder<Port> builder)
    {
        builder.Property(p => p.Code).HasMaxLength(3).IsFixedLength();
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.HasIndex(p => p.Code).IsUnique();
    }
}
