using Microsoft.EntityFrameworkCore;

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
}