using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Infrastructure.Identity.Data;
using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.Infrastructure.Identity.Data.Seeders;
using StudentParliamentSystem.Infrastructure.Identity.HostedServices;
using StudentParliamentSystem.Infrastructure.Identity.Options;

namespace StudentParliamentSystem.Infrastructure.Identity;

public static class IdentityInfrastructureServiceExtensions
{
    public static IServiceCollection AddIdentityInfrastructureServices(
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

        RegisterOptions(services, configuration);
        RegisterDatabaseContext(services, configuration);
        RegisterDataSeeders(services);
        RegisterEFRepositories(services);
        RegisterServices(services);
        ConfigureIdentity(services);

        logger.LogInformation("{Project} services registered", "Infrastructure.Identity");

        return services;
    }

    private static void RegisterDatabaseContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddDbContext<IdentityDatabaseContext>(options =>
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
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<IdentityDatabaseContext>();

        // todo implement real email confirmation, create a real email sender
        services.AddTransient<IEmailSender, NoOpEmailSender>();

        services.AddHostedService<SetupIdentityDataSeeder>();
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // todo: implement proper account confirmation flow and enable RequireConfirmedAccount
            options.SignIn.RequireConfirmedAccount = false;
        });
    }

    private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<StarterAdminAccountOptions>()
            .Bind(configuration.GetSection(StarterAdminAccountOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void RegisterDataSeeders(IServiceCollection services)
    {
        services.AddScoped<IIdentityDataSeeder, IdentityDataSeeder>();
    }
}