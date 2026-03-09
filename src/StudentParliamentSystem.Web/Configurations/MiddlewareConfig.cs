namespace StudentParliamentSystem.Api.Configurations;

public static class MiddlewareConfig
{
    public static WebApplication UseAppMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() &&
            MigrationConfig.ShouldApplyMigrationsOnStartup(app.Configuration, app.Logger))
        {
            app.UseMigrations(app.Logger);
        }
        
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
            app.MapGet("/", () => Results.Redirect("/swagger"));
        }

        return app;
    }
}