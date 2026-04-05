using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Events;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDatabaseContext _context;

    public EventRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Set<Event>().AddAsync(@event, cancellationToken);
    }
}
