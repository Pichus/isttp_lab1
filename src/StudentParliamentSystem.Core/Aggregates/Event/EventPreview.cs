namespace StudentParliamentSystem.Core.Aggregates.Event;

public record EventPreview(
    Guid Id,
    string Title,
    string Location,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    string DepartmentName,
    IEnumerable<string> Tags
);
