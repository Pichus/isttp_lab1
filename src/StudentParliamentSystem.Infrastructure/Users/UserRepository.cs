using Microsoft.EntityFrameworkCore;

using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDatabaseContext _databaseContext;

    public UserRepository(ApplicationDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public void Add(User user)
    {
        _databaseContext.Users.Add(user);
    }

    public async Task<bool> ExistsAsync(Guid userId)
    {
        return await _databaseContext.Users.AnyAsync(user => user.Id == userId);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _databaseContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _databaseContext.Users
            .Include(u => u.Roles)
            .Include(u => u.Departments)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _databaseContext.Users
            .Include(u => u.Roles)
            .Include(u => u.Departments)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId)
    {
        return await _databaseContext.Users
            .Include(u => u.Roles)
            .Include(u => u.Departments)
            .Where(u => u.Roles.Any(r => r.Id == roleId))
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds)
    {
        return await _databaseContext.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<PagedResult<UserPreview>> RetrieveAllAsync(int pageNumber, int pageSize = 10,
        string? query = null)
    {
        var databaseQuery = _databaseContext.Users.AsNoTracking();

        if (query is not null)
        {
            var trimmedQueryString = query.Trim();
            var isIdQuery = Guid.TryParse(query, out var queryGuid);

            databaseQuery = databaseQuery.Where(user => user.FirstName.Contains(trimmedQueryString) ||
                                                        user.LastName.Contains(trimmedQueryString) ||
                                                        (user.FirstName + " " + user.LastName).Contains(
                                                            trimmedQueryString) ||
                                                        user.Email.Contains(trimmedQueryString) ||
                                                        (isIdQuery && user.Id == queryGuid));
        }

        var totalCount = await databaseQuery.CountAsync();

        databaseQuery = databaseQuery
            .Include(user => user.Roles)
            .Include(user => user.Departments)
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ThenBy(user => user.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var projectedQuery = databaseQuery
            .Select(user => new UserPreview
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles.Select(role => role.Name.ToString()),
                Departments = user.Departments.Select(department => department.Name)
            });

        var result = await projectedQuery.ToListAsync();

        return new PagedResult<UserPreview>
        {
            Items = result, CurrentPage = pageNumber, PageSize = pageSize, TotalCount = totalCount
        };
    }

    public async Task<PagedResult<UserPreview>> RetrieveByDepartmentAsync(Guid departmentId, int pageNumber, int pageSize = 10,
        string? query = null)
    {
        var databaseQuery = _databaseContext.Users.AsNoTracking()
            .Where(u => u.Departments.Any(d => d.Id == departmentId));

        if (query is not null)
        {
            var trimmedQueryString = query.Trim();
            var isIdQuery = Guid.TryParse(query, out var queryGuid);

            databaseQuery = databaseQuery.Where(user => user.FirstName.Contains(trimmedQueryString) ||
                                                        user.LastName.Contains(trimmedQueryString) ||
                                                        (user.FirstName + " " + user.LastName).Contains(
                                                            trimmedQueryString) ||
                                                        user.Email.Contains(trimmedQueryString) ||
                                                        (isIdQuery && user.Id == queryGuid));
        }

        var totalCount = await databaseQuery.CountAsync();

        databaseQuery = databaseQuery
            .Include(user => user.Roles)
            .Include(user => user.Departments)
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ThenBy(user => user.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var projectedQuery = databaseQuery
            .Select(user => new UserPreview
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles.Select(role => role.Name.ToString()),
                Departments = user.Departments.Select(department => department.Name)
            });

        var result = await projectedQuery.ToListAsync();

        return new PagedResult<UserPreview>
        {
            Items = result, CurrentPage = pageNumber, PageSize = pageSize, TotalCount = totalCount
        };
    }
}