using StudentParliamentSystem.Core.Abstractions;

namespace StudentParliamentSystem.Core.Aggregates.User;

public interface IUserRepository
{
    void Add(User user);
    Task<bool> ExistsAsync(Guid userId);
    Task<bool> ExistsAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task<PagedResult<UserPreview>> RetrieveAllAsync(int pageNumber, int pageSize, string? query = null);
}