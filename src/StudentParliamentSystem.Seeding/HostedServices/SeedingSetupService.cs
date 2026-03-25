using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Seeding.Seeders;

namespace StudentParliamentSystem.Seeding.HostedServices;

public class SeedingSetupService : IHostedService
{
    private readonly ILogger<SeedingSetupService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SeedingSetupService(ILogger<SeedingSetupService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start seeding");
        using var scope = _scopeFactory.CreateScope();
        
        var roleSeeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
        
        await roleSeeder.SeedAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}