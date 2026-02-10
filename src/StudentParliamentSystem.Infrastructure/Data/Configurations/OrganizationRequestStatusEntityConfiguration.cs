using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.OrganizationRequest;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class OrganizationRequestStatusEntityConfiguration : IEntityTypeConfiguration<OrganizationRequestStatus>
{
    public void Configure(EntityTypeBuilder<OrganizationRequestStatus> builder)
    {
        builder
            .ToTable("organization_request_statuses");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}