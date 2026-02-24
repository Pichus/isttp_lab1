using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;

namespace StudentParliamentSystem.Infrastructure.Identity.Data;

public class IdentityDatabaseContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    protected IdentityDatabaseContext()
    {
    }

    public IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDatabaseContext).Assembly);
    }
}