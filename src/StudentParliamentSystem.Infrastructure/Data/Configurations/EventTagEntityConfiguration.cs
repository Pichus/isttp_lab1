using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class EventTagEntityConfiguration : IEntityTypeConfiguration<EventTag>
{
    public void Configure(EntityTypeBuilder<EventTag> builder)
    {
        builder
            .ToTable("event_tags");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}