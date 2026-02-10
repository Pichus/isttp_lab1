using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class DepartmentEntityConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder
            .ToTable("departments");

        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(e => e.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder
            .HasMany(e => e.Users)
            .WithMany(e => e.Departments)
            .UsingEntity<DepartmentMember>(
                r => r
                    .HasOne<User>(e => e.User)
                    .WithMany(e => e.DepartmentMembers),
                l => l
                    .HasOne<Department>(e => e.Department)
                    .WithMany(e => e.DepartmentMembers));
    }
}