using FluentResults;

namespace StudentParliamentSystem.UseCases.Departments.Update;

public record ChangeDepartmentHead(Guid DepartmentId, string Email);