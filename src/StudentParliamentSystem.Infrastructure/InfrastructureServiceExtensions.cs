using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.Data;
using StudentParliamentSystem.Infrastructure.Roles;
using StudentParliamentSystem.Infrastructure.Users;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string environmentName, ILogger logger)
    {
        if (environmentName == "Development")
        {
            RegisterDevelopmentOnlyDependencies(services, configuration);
        }
        else if (environmentName == "Testing")
        {
            RegisterTestingOnlyDependencies(services);
        }
        else
        {
            RegisterProductionOnlyDependencies(services, configuration);
        }

        AddDbContextWithPostgres(services, configuration);
        RegisterEFRepositories(services);
        RegisterServices(services);

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }

    private static void AddDbContextWithPostgres(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddDbContext<ApplicationDatabaseContext>(options =>
            options.UseNpgsql(connectionString));
    }


    private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void RegisterTestingOnlyDependencies(IServiceCollection services)
    {
    }

    private static void RegisterProductionOnlyDependencies(IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void RegisterEFRepositories(IServiceCollection services)
    {
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}