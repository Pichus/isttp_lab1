using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class CoworkingBookingStatusEntityConfiguration : IEntityTypeConfiguration<CoworkingBookingStatus>
{
    public void Configure(EntityTypeBuilder<CoworkingBookingStatus> builder)
    {
        builder
            .ToTable("coworking_statuses");

        builder
            .HasKey(e => e.Id);
        
        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}