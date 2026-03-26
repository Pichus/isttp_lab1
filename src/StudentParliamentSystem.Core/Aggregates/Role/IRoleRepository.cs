using FluentResults;

namespace StudentParliamentSystem.Core.Aggregates.Role;

public interface IRoleRepository
{
    Task<Result<Role>> GetByNameAsync(RoleName name);
    void Create(Role role);
}