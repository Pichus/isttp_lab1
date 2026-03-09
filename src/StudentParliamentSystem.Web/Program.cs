using StudentParliamentSystem.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddServices(builder.Environment, builder.Configuration);

var app = builder.Build();

app.UseAppMiddleware();

app.Run();