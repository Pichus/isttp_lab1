using Microsoft.EntityFrameworkCore;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.CoworkingBookings;

public class CoworkingBookingRepository : ICoworkingBookingRepository
{
    private readonly ApplicationDatabaseContext _context;

    public CoworkingBookingRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CoworkingBooking booking, CancellationToken cancellationToken = default)
    {
        await _context.Set<CoworkingBooking>().AddAsync(booking, cancellationToken);
    }

    public async Task<CoworkingBooking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CoworkingBooking>()
            .Include(b => b.Status)
            .Include(b => b.Event)
                .ThenInclude(e => e.CreatedByUser)
            .Include(b => b.SpaceManager)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<PagedResult<CoworkingBookingPreview>> GetAllAsync(
        int pageNumber, int pageSize, string? statusFilter, CancellationToken cancellationToken = default)
    {
        var q = _context.Set<CoworkingBooking>()
            .Include(b => b.Status)
            .Include(b => b.Event)
                .ThenInclude(e => e.CreatedByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter))
            q = q.Where(b => b.Status.Name == statusFilter);

        q = q.OrderByDescending(b => b.StartTimeUtc);

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new CoworkingBookingPreview(
                b.Id,
                b.Event.Title,
                b.Event.StartTimeUtc,
                b.Event.EndTimeUtc,
                b.StartTimeUtc,
                b.EndTimeUtc,
                b.Status.Name,
                b.Event.CreatedByUser.FirstName + " " + b.Event.CreatedByUser.LastName,
                b.Notes
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<CoworkingBookingPreview>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<CoworkingBookingSlot>> GetScheduleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<CoworkingBooking>()
            .Include(b => b.Status)
            .Include(b => b.Event)
            .Where(b => b.Status.Name != "Rejected")
            .OrderBy(b => b.StartTimeUtc)
            .Select(b => new CoworkingBookingSlot(
                b.Event.Title,
                b.StartTimeUtc,
                b.EndTimeUtc,
                b.Status.Name
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<CoworkingBookingStatus?> GetStatusByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CoworkingBookingStatus>()
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }
}
