using StudentParliamentSystem.Api.Configurations;
using StudentParliamentSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddInfrastructureServices(builder.Configuration,
    builder.Environment.EnvironmentName);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAppMiddleware();

app.Run();