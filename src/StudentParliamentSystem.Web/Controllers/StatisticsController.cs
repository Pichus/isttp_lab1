using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentParliamentSystem.UseCases.Statistics.GetOverallStatistics;

using Wolverine;

namespace StudentParliamentSystem.Api.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly IMessageBus _bus;

    public StatisticsController(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task<IActionResult> Index([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var stats = await _bus.InvokeAsync<StudentParliamentSystem.Core.Aggregates.Statistics.OverallStatistics>(new GetOverallStatisticsQuery(startDate, endDate), cancellationToken);

        var model = new StatisticsViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            ActiveMembersCount = stats.ActiveMembersCount,
            ConductedEventsCount = stats.ConductedEventsCount,
            CoworkingHours = stats.CoworkingHours,
            DepartmentActivities = stats.DepartmentActivities.Select(d => new DepartmentActivityVm
            {
                Name = d.Name,
                EventsCount = d.EventsCount
            }).ToList(),
            TopEvents = stats.TopEvents.Select(e => new TopEventVm
            {
                Name = e.Name,
                ParticipantsCount = e.ParticipantsCount,
                DepartmentName = e.DepartmentName
            }).ToList()
        };

        return View(model);
    }
}

public class StatisticsViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int ActiveMembersCount { get; set; }
    public int ConductedEventsCount { get; set; }
    public int CoworkingHours { get; set; }
    public List<DepartmentActivityVm> DepartmentActivities { get; set; } = new();
    public List<TopEventVm> TopEvents { get; set; } = new();
}

public class DepartmentActivityVm
{
    public string Name { get; set; } = string.Empty;
    public int EventsCount { get; set; }
}

public class TopEventVm
{
    public string Name { get; set; } = string.Empty;
    public int ParticipantsCount { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}