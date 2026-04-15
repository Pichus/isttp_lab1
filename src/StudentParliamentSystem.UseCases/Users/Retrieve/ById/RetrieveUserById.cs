using FluentResults;

using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.UseCases.Users.Retrieve.ById;

public record RetrieveUserById(Guid UserId);

public class RetrieveUserByIdHandler
{
    private readonly IUserRepository _userRepository;

    public RetrieveUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> HandleAsync(RetrieveUserById query)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);

        if (user is null)
        {
            return Result.Fail($"User with ID {query.UserId} not found");
        }

        return Result.Ok(user);
    }
}