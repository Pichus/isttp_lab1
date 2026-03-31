using FluentResults;

using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.Role.Errors;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Roles;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDatabaseContext _context;

    public RoleRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<Role>> GetByNameAsync(RoleName name)
    {
        var result = await _context.Roles.FirstOrDefaultAsync(role => role.Name == name);

        if (result is null)
        {
            return Result.Fail(new RoleNotFoundError(name.ToString()));
        }

        return Result.Ok(result);
    }

    public async Task<IEnumerable<Role>> GetByNamesAsync(IEnumerable<RoleName> names)
    {
        return await _context.Roles
            .Where(role => names.Contains(role.Name))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(RoleName name)
    {
        return await _context.Roles.AnyAsync(role => role.Name == name);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Roles.AnyAsync(role => role.Id == id);
    }

    public void Create(Role role)
    {
        _context.Roles.Add(role);
    }
}