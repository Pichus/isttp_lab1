using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;

namespace StudentParliamentSystem.Infrastructure.Identity.Data.Configurations;

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.FirstName)
            .HasMaxLength(35)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(35)
            .IsRequired();
    }
}