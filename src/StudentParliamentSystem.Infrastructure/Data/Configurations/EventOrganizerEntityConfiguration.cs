using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class EventOrganizerEntityConfiguration : IEntityTypeConfiguration<EventOrganizer>
{
    public void Configure(EntityTypeBuilder<EventOrganizer> builder)
    {
        builder
            .ToTable("event_organizers");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.RoleDescription)
            .HasMaxLength(1000);

        builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder
            .HasIndex(e => new { e.EventId, e.UserId })
            .IsUnique();
    }
}