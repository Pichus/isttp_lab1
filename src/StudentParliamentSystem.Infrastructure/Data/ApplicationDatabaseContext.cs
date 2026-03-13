using Microsoft.EntityFrameworkCore;

namespace StudentParliamentSystem.Infrastructure.Data;

public class ApplicationDatabaseContext : DbContext
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

        modelBuilder.HasDefaultSchema("main_application");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDatabaseContext).Assembly);
    }
}