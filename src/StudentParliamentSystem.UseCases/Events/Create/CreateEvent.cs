namespace StudentParliamentSystem.UseCases.Events.Create;

public record CreateEvent(
    string Title,
    string Description,
    string Location,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    Guid DepartmentId,
    int? MaxParticipants,
    bool IsPublished,
    List<string> Tags,
    Guid CreatedByUserId
);