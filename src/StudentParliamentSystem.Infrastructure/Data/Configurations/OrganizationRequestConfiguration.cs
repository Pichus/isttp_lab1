using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.OrganizationRequest;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class OrganizationRequestConfiguration : IEntityTypeConfiguration<OrganizationRequest>
{
    public void Configure(EntityTypeBuilder<OrganizationRequest> builder)
    {
        builder
            .ToTable("organization_requests");

        builder
            .HasKey(e => e.Id);

        builder
            .HasIndex(e => new { e.EventId, e.UserId })
            .IsUnique();

        builder
            .HasOne(e => e.Status)
            .WithMany(e => e.OrganizationRequests)
            .HasForeignKey(e => e.StatusId)
            .IsRequired();

        builder
            .Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder
            .HasOne(e => e.User)
            .WithMany(e => e.OrganizationRequests)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder
            .HasOne(e => e.Event)
            .WithMany(e => e.OrganizationRequests)
            .HasForeignKey(e => e.EventId)
            .IsRequired();
    }
}