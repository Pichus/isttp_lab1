using JasperFx.Core;
using JasperFx.Resources;

using StudentParliamentSystem.UseCases;

using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

namespace StudentParliamentSystem.Api.Configurations;

public static class WolverineConfig
{
    public static void AddWolverineConfig(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        if (connectionString is null)
        {
            throw new InvalidOperationException(
                "No connection string configured for postgres db, can't configure Wolverine.");
        }

        hostBuilder.UseWolverine(options =>
        {
            options.PersistMessagesWithPostgresql(connectionString, "message_bus_outbox");
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.UseDurableLocalQueues();
            options.Durability.KeepAfterMessageHandling = TimeSpan.FromHours(1);
            options.Durability.OutboxStaleTime = 1.Hours();
            options.Durability.InboxStaleTime = 10.Minutes();
            options.Durability.MessageIdentity = MessageIdentity.IdAndDestination;
            options.Discovery.IncludeAssembly(typeof(UseCaseServiceExtensions).Assembly);
        });

        hostBuilder.UseResourceSetupOnStartup();
    }
}