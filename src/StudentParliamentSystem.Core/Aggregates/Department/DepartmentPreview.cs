namespace StudentParliamentSystem.Core.Aggregates.Department;

public record DepartmentPreview(
    Guid Id,
    string Name,
    string Description,
    string? HeadName,
    int MemberCount);