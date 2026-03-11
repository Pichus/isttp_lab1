using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Infrastructure.Identity;

namespace StudentParliamentSystem.Infrastructure.Data;

public class ApplicationDatabaseContext : IdentityDbContext<StudentParliamentSystemUser, IdentityRole<Guid>, Guid>
{
    protected ApplicationDatabaseContext()
    {
    }

    public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDatabaseContext).Assembly);
    }
}