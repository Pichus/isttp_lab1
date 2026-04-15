namespace StudentParliamentSystem.UseCases.Events.Retrieve.Published;

public record RetrievePublishedEvents(
    int PageNumber = 1,
    int PageSize = 10,
    string? Query = null,
    string? Tag = null,
    string? SortOrder = "date_desc"
);