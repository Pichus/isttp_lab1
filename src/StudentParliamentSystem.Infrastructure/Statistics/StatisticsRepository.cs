using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.Statistics;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Statistics;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly ApplicationDatabaseContext _context;

    public StatisticsRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<OverallStatistics> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        if (startDate.HasValue && startDate.Value.Kind == DateTimeKind.Unspecified)
            startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            
        if (endDate.HasValue && endDate.Value.Kind == DateTimeKind.Unspecified)
            endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);

        var usersQuery = _context.Users.AsQueryable();
        if (startDate.HasValue) usersQuery = usersQuery.Where(u => u.CreatedAtUtc >= startDate.Value);
        if (endDate.HasValue) usersQuery = usersQuery.Where(u => u.CreatedAtUtc <= endDate.Value);
        var activeMembersCount = await usersQuery.CountAsync(cancellationToken);

        var eventsQuery = _context.Set<Event>().AsQueryable();
        if (startDate.HasValue) eventsQuery = eventsQuery.Where(e => e.StartTimeUtc >= startDate.Value);
        if (endDate.HasValue) eventsQuery = eventsQuery.Where(e => e.StartTimeUtc <= endDate.Value);
        
        var conductedEventsCount = await eventsQuery.CountAsync(cancellationToken);

        var bookingsQuery = _context.CoworkingBookings.AsQueryable();
        if (startDate.HasValue) bookingsQuery = bookingsQuery.Where(b => b.StartTimeUtc >= startDate.Value);
        if (endDate.HasValue) bookingsQuery = bookingsQuery.Where(b => b.StartTimeUtc <= endDate.Value);

        var bookings = await bookingsQuery
            .Select(b => new { b.StartTimeUtc, b.EndTimeUtc })
            .ToListAsync(cancellationToken);

        var coworkingHours = bookings.Sum(b => (b.EndTimeUtc - b.StartTimeUtc).TotalHours);

        var departmentActivities = await _context.Departments
            .Select(d => new DepartmentActivityStats(
                d.Name,
                eventsQuery.Count(e => e.DepartmentId == d.Id)
            ))
            .ToListAsync(cancellationToken);

        var topEvents = await eventsQuery
            .OrderByDescending(e => e.Registrations.Count)
            .Take(5)
            .Select(e => new TopEventStats(
                e.Title,
                e.Registrations.Count,
                e.Department.Name
            ))
            .ToListAsync(cancellationToken);

        return new OverallStatistics(
            activeMembersCount,
            conductedEventsCount,
            (int)Math.Round(coworkingHours),
            departmentActivities,
            topEvents
        );
    }
}