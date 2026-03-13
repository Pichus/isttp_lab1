using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Infrastructure.Identity.Data.Seeders;

namespace StudentParliamentSystem.Infrastructure.Identity.HostedServices;

public class SetupIdentityDataSeeder : IHostedService
{
    private readonly ILogger<SetupIdentityDataSeeder> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SetupIdentityDataSeeder(ILogger<SetupIdentityDataSeeder> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start identity database seeding");
        using var scope = _scopeFactory.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IIdentityDataSeeder>();
        await seeder.SeedAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}