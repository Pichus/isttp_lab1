using StudentParliamentSystem.Core.Abstractions;

namespace StudentParliamentSystem.Core.Aggregates.User;

public interface IUserRepository
{
    void Add(User user);
    Task<bool> ExistsAsync(Guid userId);
    Task<bool> ExistsAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId);
    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds);
    Task<PagedResult<UserPreview>> RetrieveAllAsync(int pageNumber, int pageSize, string? query = null);
    Task<PagedResult<UserPreview>> RetrieveByDepartmentAsync(Guid departmentId, int pageNumber, int pageSize, string? query = null);
}