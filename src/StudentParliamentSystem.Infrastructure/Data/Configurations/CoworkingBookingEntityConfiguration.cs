using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class CoworkingBookingEntityConfiguration : IEntityTypeConfiguration<CoworkingBooking>
{
    public void Configure(EntityTypeBuilder<CoworkingBooking> builder)
    {
        builder
            .ToTable("coworking_bookings");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.StartTimeUtc)
            .IsRequired();
        
        builder
            .Property(e => e.EndTimeUtc)
            .IsRequired();

        builder
            .Property(e => e.Notes)
            .HasMaxLength(1000);

        builder
            .HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventId)
            .IsRequired();

        builder
            .HasOne(e => e.SpaceManager)
            .WithMany()
            .HasForeignKey(e => e.SpaceManagerId);
        
        builder
            .HasOne(e => e.Status)
            .WithMany()
            .HasForeignKey(e => e.StatusId)
            .IsRequired();
    }
}