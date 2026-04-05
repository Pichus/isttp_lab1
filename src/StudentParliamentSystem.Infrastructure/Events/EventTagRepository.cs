using Microsoft.EntityFrameworkCore;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Events;

public class EventTagRepository : IEventTagRepository
{
    private readonly ApplicationDatabaseContext _context;

    public EventTagRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<EventTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Set<EventTag>()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<EventTag>> RetrieveAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<EventTag>()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EventTag eventTag, CancellationToken cancellationToken = default)
    {
        await _context.Set<EventTag>().AddAsync(eventTag, cancellationToken);
    }
}
