namespace StudentParliamentSystem.Core.Aggregates.User;

public interface IUserRepository
{
    void Add(User user);
    Task<bool> ExistsAsync(Guid userId);
}