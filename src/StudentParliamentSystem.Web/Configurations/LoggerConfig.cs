using Serilog;

namespace StudentParliamentSystem.Api.Configurations;

public static class LoggerConfig
{
    public static IServiceCollection AddLoggerConfig(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddSerilog(new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", environment.ApplicationName)
            .CreateLogger());

        return services;
    }
}