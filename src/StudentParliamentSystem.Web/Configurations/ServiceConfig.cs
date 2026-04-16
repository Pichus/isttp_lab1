using StudentParliamentSystem.Infrastructure;
using StudentParliamentSystem.Infrastructure.Identity;
using StudentParliamentSystem.Seeding;

namespace StudentParliamentSystem.Api.Configurations;

public static class ServiceConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services, IWebHostEnvironment environment,
        IConfiguration configuration, ILogger logger)
    {
        services.AddControllersWithViews();

        services.AddLoggerConfig(configuration, environment);

        services.AddAuthorizationConfig();

        services.AddRazorPages();

        services.AddSwaggerConfig();

        services
            .AddInfrastructureServices(configuration, environment.EnvironmentName, logger)
            .AddIdentityInfrastructureServices(configuration, environment.EnvironmentName, logger)
            .AddSeeding(configuration, logger);

        services.AddAuthenticationConfig();

        return services;
    }
}