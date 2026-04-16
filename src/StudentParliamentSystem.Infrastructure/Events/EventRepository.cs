using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Abstractions;
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

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Event>()
            .Include(e => e.Department)
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<PagedResult<EventPreview>> RetrievePublishedAsync(
        int pageNumber,
        int pageSize,
        string? query,
        string? tag,
        string? sortOrder,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Set<Event>()
            .Include(e => e.Department)
            .Include(e => e.Tags)
            .Where(e => e.IsPublished);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            q = q.Where(e => e.Title.ToLower().Contains(lowerQuery) ||
                             e.Description.ToLower().Contains(lowerQuery) ||
                             e.Location.ToLower().Contains(lowerQuery));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            q = q.Where(e => e.Tags.Any(t => t.Name == tag));
        }

        if (sortOrder == "date_desc")
            q = q.OrderByDescending(e => e.StartTimeUtc);
        else if (sortOrder == "date_asc")
            q = q.OrderBy(e => e.StartTimeUtc);
        else
            q = q.OrderByDescending(e => e.StartTimeUtc);

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventPreview(
                e.Id,
                e.Title,
                e.Location,
                e.StartTimeUtc,
                e.EndTimeUtc,
                e.Department!.Name,
                e.Tags.Select(t => t.Name)
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<EventPreview>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<EventPreview>> RetrieveByDepartmentAsync(
        Guid departmentId,
        int pageNumber,
        int pageSize,
        string? query,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Set<Event>()
            .Include(e => e.Department)
            .Include(e => e.Tags)
            .Where(e => e.DepartmentId == departmentId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            q = q.Where(e => e.Title.ToLower().Contains(lowerQuery) ||
                             e.Description.ToLower().Contains(lowerQuery) ||
                             e.Location.ToLower().Contains(lowerQuery));
        }

        q = q.OrderByDescending(e => e.StartTimeUtc);

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventPreview(
                e.Id,
                e.Title,
                e.Location,
                e.StartTimeUtc,
                e.EndTimeUtc,
                e.Department!.Name,
                e.Tags.Select(t => t.Name)
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<EventPreview>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public Task DeleteAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Set<Event>().Remove(@event);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Event>> GetByCreatedByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Event>()
            .Where(e => e.CreatedByUserId == userId)
            .OrderByDescending(e => e.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetByIdWithRegistrationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Event>()
            .Include(e => e.Department)
            .Include(e => e.Tags)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddRegistrationAsync(EventRegistration registration, CancellationToken cancellationToken = default)
    {
        await _context.Set<EventRegistration>().AddAsync(registration, cancellationToken);
    }

    public Task RemoveRegistrationAsync(EventRegistration registration, CancellationToken cancellationToken = default)
    {
        _context.Set<EventRegistration>().Remove(registration);
        return Task.CompletedTask;
    }
}