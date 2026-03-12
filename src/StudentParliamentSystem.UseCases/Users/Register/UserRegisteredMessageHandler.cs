using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Shared.Contracts.Users;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Users.Register;

public class UserRegisteredHandler
{
    private readonly ILogger<UserRegisteredHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserRegisteredHandler(IUserRepository userRepository, ILogger<UserRegisteredHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UserRegistered message)
    {
        var userExists = await _userRepository.ExistsAsync(message.UserId);

        if (userExists)
        {
            _logger.LogInformation(
                $"User with ID {message.UserId} already exists. UserCreated message was sent twice.");
            return;
        }

        var user = User.Create(message.UserId, message.Email, message.FirstName, message.LastName);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Created User with ID {message.UserId} successfully.");
    }
}