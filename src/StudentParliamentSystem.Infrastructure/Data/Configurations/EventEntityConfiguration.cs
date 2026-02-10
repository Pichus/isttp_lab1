using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class EventEntityConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .ToTable("events");
        
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .Property(e => e.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder
            .Property(e => e.PosterUrl)
            .HasDefaultValue(null)
            .HasMaxLength(500);

        builder
            .Property(e => e.Location)
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(e => e.MaxParticipants)
            .HasDefaultValue(null);

        builder
            .Property(e => e.IsPublished)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder
            .HasOne(e => e.CreatedByUser)
            .WithMany(e => e.CreatedEvents)
            .HasForeignKey(e => e.CreatedByUserId)
            .IsRequired();

        builder
            .HasMany(e => e.EventOrganizers)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventId)
            .IsRequired();

        builder
            .HasMany(e => e.Tags)
            .WithMany(e => e.Events)
            .UsingEntity(e => e.ToTable("events_tags"));

        builder
            .HasMany(e => e.Registrations)
            .WithOne(e => e.Event)
            .HasForeignKey(e => e.EventId)
            .IsRequired();
    }
}