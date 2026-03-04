namespace StudentParliamentSystem.Api.Configurations;

public static class AuthorizationConfig
{
    public static IServiceCollection AddAuthorizationConfig(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddAuthorizationPolicies();
        });

        return services;
    }
}