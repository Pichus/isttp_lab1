using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Seeding.HostedServices;
using StudentParliamentSystem.Seeding.Options;
using StudentParliamentSystem.Seeding.Seeders;

namespace StudentParliamentSystem.Seeding;

public static class SeedingServiceExtensions
{
    public static IServiceCollection AddSeeding(this IServiceCollection services, IConfiguration configuration,
        ILogger logger)
    {
        RegisterOptions(services, configuration);
        RegisterDataSeeders(services);

        logger.LogInformation("{Project} services registered", "Seeding");

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddHostedService<SeedingSetupService>();
    }

    private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<InitialAdminAccountOptions>()
            .Bind(configuration.GetSection(InitialAdminAccountOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void RegisterDataSeeders(IServiceCollection services)
    {
        services.AddScoped<IRoleSeeder, RoleSeeder>();
    }
}