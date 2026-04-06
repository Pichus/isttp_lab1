namespace StudentParliamentSystem.UseCases.Events.Update;

public record UpdateEvent(
    Guid EventId,
    string Title,
    string Description,
    string Location,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    Guid DepartmentId,
    int? MaxParticipants,
    bool IsPublished,
    IEnumerable<string> Tags,
    Guid RequestingUserId
);
