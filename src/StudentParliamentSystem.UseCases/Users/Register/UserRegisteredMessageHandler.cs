using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Shared.Contracts.Users;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Users.Register;

public class UserRegisteredHandler
{
    private readonly ILogger<UserRegisteredHandler> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserRegisteredHandler(IUserRepository userRepository, ILogger<UserRegisteredHandler> logger,
        IUnitOfWork unitOfWork, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
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

        var roles = new List<Role>();

        foreach (var roleName in message.Roles)
        {
            var parseRoleNameResult = Enum.TryParse<RoleName>(roleName, out var parsedRoleName);

            if (!parseRoleNameResult)
            {
                _logger.LogWarning($"Role with name {roleName} doesn't exist");
                continue;
            }

            var getRoleResult = await _roleRepository.GetByNameAsync(parsedRoleName);

            if (getRoleResult.IsFailed)
            {
                foreach (var error in getRoleResult.Errors)
                {
                    _logger.LogWarning(error.Message);
                }
                continue;
            }

            roles.Add(getRoleResult.Value);
        }

        user.Roles = roles;

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Created User with ID {message.UserId} successfully.");
    }
}