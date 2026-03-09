using Serilog;
using Serilog.Events;

using StudentParliamentSystem.Api.Configurations;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
    var startupLogger = loggerFactory.CreateLogger<Program>();

    builder.Services.AddServices(builder.Environment, builder.Configuration, startupLogger);

    var app = builder.Build();

    app.UseAppMiddleware();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}