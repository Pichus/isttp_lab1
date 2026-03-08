using StudentParliamentSystem.Infrastructure;
using StudentParliamentSystem.Infrastructure.Identity;

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
            .AddIdentityInfrastructureServices(configuration, environment.EnvironmentName, logger);
        
        services.AddAuthenticationConfig();

        return services;
    }
}