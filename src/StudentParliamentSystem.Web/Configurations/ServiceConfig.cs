using StudentParliamentSystem.Infrastructure;

namespace StudentParliamentSystem.Api.Configurations;

public static class ServiceConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services, IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        services.AddSwaggerConfig();
        
        services.AddInfrastructureServices(configuration,
            environment.EnvironmentName);

        return services;
    }
}