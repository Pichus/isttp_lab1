using FluentResults;

namespace StudentParliamentSystem.Core.Aggregates.Role.Errors;

public class RoleNotFoundError : Error
{
    public RoleNotFoundError(string roleName)
        : base($"Role with name {roleName} was not found.")
    {
    }
}