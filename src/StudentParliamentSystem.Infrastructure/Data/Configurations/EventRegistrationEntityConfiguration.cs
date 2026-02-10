using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class EventRegistrationEntityConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder
            .ToTable("event_registrations");

        builder
            .HasKey(e => e.Id);

        builder
            .HasIndex(e => new { e.EventId, e.UserId })
            .IsUnique();
    }
}