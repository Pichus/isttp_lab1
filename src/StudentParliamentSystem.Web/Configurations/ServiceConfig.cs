using StudentParliamentSystem.Infrastructure;

namespace StudentParliamentSystem.Api.Configurations;

public static class ServiceConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services, IWebHostEnvironment environment,
        IConfiguration configuration, ILogger logger)
    {
        services.AddControllersWithViews();

        services.AddLoggerConfig(configuration, environment);

        services.AddSwaggerConfig();

        services.AddInfrastructureServices(configuration,
            environment.EnvironmentName, logger);

        return services;
    }
}