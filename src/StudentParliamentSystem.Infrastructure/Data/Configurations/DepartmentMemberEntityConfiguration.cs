using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Core.Aggregates.Department;

namespace StudentParliamentSystem.Infrastructure.Data.Configurations;

public class DepartmentMemberEntityConfiguration : IEntityTypeConfiguration<DepartmentMember>
{
    public void Configure(EntityTypeBuilder<DepartmentMember> builder)
    {
        builder
            .ToTable("department_members");
        
        builder
            .HasAlternateKey(e => new { e.UserId, e.DepartmentId });

        builder
            .HasOne(e => e.DepartmentRole)
            .WithMany(e => e.DepartmentMembers)
            .HasForeignKey(e => e.DepartmentRoleId)
            .IsRequired();
    }
}