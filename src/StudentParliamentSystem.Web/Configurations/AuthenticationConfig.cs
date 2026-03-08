namespace StudentParliamentSystem.Api.Configurations;

public static class AuthenticationConfig
{
    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.LogoutPath = "/Identity/Account/Logout";
        });
        
        return services;
    }
}