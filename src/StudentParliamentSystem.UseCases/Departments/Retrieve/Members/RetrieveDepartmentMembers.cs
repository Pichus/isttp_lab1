using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.UseCases.Departments.Retrieve.Members;

public record RetrieveDepartmentMembers(Guid DepartmentId, int PageNumber, int PageSize, string? Query);
