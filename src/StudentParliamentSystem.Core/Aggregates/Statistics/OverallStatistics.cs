namespace StudentParliamentSystem.Core.Aggregates.Statistics;

public record DepartmentActivityStats(
    string Name,
    int EventsCount
);

public record TopEventStats(
    string Name,
    int ParticipantsCount,
    string DepartmentName
);

public record OverallStatistics(
    int ActiveMembersCount,
    int ConductedEventsCount,
    int CoworkingHours,
    IEnumerable<DepartmentActivityStats> DepartmentActivities,
    IEnumerable<TopEventStats> TopEvents
);
