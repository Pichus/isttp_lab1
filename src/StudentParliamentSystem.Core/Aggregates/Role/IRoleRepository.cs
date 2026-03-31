using FluentResults;

namespace StudentParliamentSystem.Core.Aggregates.Role;

public interface IRoleRepository
{
    Task<Result<Role>> GetByNameAsync(RoleName name);
    Task<IEnumerable<Role>> GetByNamesAsync(IEnumerable<RoleName> names);
    Task<bool> ExistsAsync(RoleName name);
    Task<bool> ExistsAsync(Guid id);
    void Create(Role role);
}