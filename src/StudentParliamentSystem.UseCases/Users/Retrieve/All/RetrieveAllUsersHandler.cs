using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.UseCases.Users.Retrieve.All;

public class RetrieveAllUsersHandler
{
    private readonly IUserRepository _userRepository;

    public RetrieveAllUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserPreview>> HandleAsync(RetrieveAllUsers query)
    {
        var result = await _userRepository.RetrieveAllAsync(
            query.PageNumber, query.PageSize, query.Query);

        return result;
    }
}