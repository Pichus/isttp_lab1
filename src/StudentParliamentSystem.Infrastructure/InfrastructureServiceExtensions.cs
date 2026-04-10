using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.CoworkingBookings;
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
        services.AddScoped<StudentParliamentSystem.Core.Aggregates.Department.IDepartmentRepository, StudentParliamentSystem.Infrastructure.Departments.DepartmentRepository>();
        services.AddScoped<StudentParliamentSystem.Core.Aggregates.Event.IEventRepository, StudentParliamentSystem.Infrastructure.Events.EventRepository>();
        services.AddScoped<StudentParliamentSystem.Core.Aggregates.Event.IEventTagRepository, StudentParliamentSystem.Infrastructure.Events.EventTagRepository>();
        services.AddScoped<StudentParliamentSystem.Core.Aggregates.CoworkingBooking.ICoworkingBookingRepository, CoworkingBookingRepository>();
        services.AddScoped<StudentParliamentSystem.Core.Aggregates.Statistics.IStatisticsRepository, StudentParliamentSystem.Infrastructure.Statistics.StatisticsRepository>();
        services.AddScoped<StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument.ICoworkingDocumentGenerator, StudentParliamentSystem.Infrastructure.CoworkingBookings.CoworkingDocumentGenerator>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}