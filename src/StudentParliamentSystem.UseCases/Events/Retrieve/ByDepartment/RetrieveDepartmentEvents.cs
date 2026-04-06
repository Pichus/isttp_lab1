namespace StudentParliamentSystem.UseCases.Events.Retrieve.ByDepartment;

public record RetrieveDepartmentEvents(
    Guid DepartmentId,
    int PageNumber = 1,
    int PageSize = 10,
    string? Query = null
);
