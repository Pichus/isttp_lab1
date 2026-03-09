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

        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
            app.MapGet("/", () => Results.Redirect("/swagger"));
        }

        return app;
    }
}