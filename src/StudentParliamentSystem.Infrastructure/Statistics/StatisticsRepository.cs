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

    public async Task<OverallStatistics> GetOverallStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var activeMembersCount = await _context.Users.CountAsync(cancellationToken);

        var conductedEventsCount = await _context.Set<Event>().CountAsync(cancellationToken);

        var bookings = await _context.CoworkingBookings
            .Select(b => new { b.StartTimeUtc, b.EndTimeUtc })
            .ToListAsync(cancellationToken);

        var coworkingHours = bookings.Sum(b => (b.EndTimeUtc - b.StartTimeUtc).TotalHours);

        var departmentActivities = await _context.Departments
            .Select(d => new DepartmentActivityStats(
                d.Name,
                _context.Set<Event>().Count(e => e.DepartmentId == d.Id)
            ))
            .ToListAsync(cancellationToken);

        var topEvents = await _context.Set<Event>()
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