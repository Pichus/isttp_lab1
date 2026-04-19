using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Department;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class DepartmentRoleEntityConfiguration : IEntityTypeConfiguration<DepartmentRole>
{
    public void Configure(EntityTypeBuilder<DepartmentRole> builder)
    {
        builder
            .ToTable("department_roles");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
    }
}