using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Role;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .ToTable("roles");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}