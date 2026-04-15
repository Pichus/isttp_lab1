using FluentResults;

using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Shared.Contracts.Users;
using StudentParliamentSystem.UseCases.Abstractions;

using Wolverine;

namespace StudentParliamentSystem.UseCases.Users.Update;

public record UpdateUserDetails(Guid UserId, string FirstName, string LastName, ICollection<string> Roles);

public class UpdateUserDetailsHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _bus;
    private readonly ILogger<UpdateUserDetailsHandler> _logger;

    public UpdateUserDetailsHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        IMessageBus bus,
        ILogger<UpdateUserDetailsHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _bus = bus;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateUserDetails command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user is null)
        {
            return Result.Fail($"User with ID {command.UserId} not found");
        }

        user.FirstName = command.FirstName;
        user.LastName = command.LastName;

        var roles = new List<Role>();
        foreach (var roleName in command.Roles)
        {
            if (!Enum.TryParse<RoleName>(roleName, out var parsedRoleName))
            {
                _logger.LogWarning($"Role with name {roleName} doesn't exist");
                continue;
            }

            var getRoleResult = await _roleRepository.GetByNameAsync(parsedRoleName);
            if (getRoleResult.IsFailed)
            {
                _logger.LogWarning($"Failed to get role: {parsedRoleName}");
                continue;
            }

            roles.Add(getRoleResult.Value);
        }

        user.Roles = roles;

        await _unitOfWork.SaveChangesAsync();

        await _bus.PublishAsync(new UserDetailsUpdated(user.Id, user.FirstName, user.LastName, user.Roles.Select(r => r.Name.ToString()).ToList()));

        _logger.LogInformation($"Updated User with ID {command.UserId} successfully.");

        return Result.Ok();
    }
}