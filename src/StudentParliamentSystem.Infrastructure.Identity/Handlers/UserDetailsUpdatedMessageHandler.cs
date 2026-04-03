using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.Shared.Contracts.Users;

namespace StudentParliamentSystem.Infrastructure.Identity.Handlers;

public class UserDetailsUpdatedMessageHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserDetailsUpdatedMessageHandler> _logger;

    public UserDetailsUpdatedMessageHandler(UserManager<ApplicationUser> userManager, ILogger<UserDetailsUpdatedMessageHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task HandleAsync(UserDetailsUpdated message)
    {
        var user = await _userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null)
        {
            _logger.LogWarning($"User with ID {message.UserId} not found in Identity database. Cannot update details.");
            return;
        }

        user.FirstName = message.FirstName;
        user.LastName = message.LastName;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            _logger.LogError($"Failed to update details for Identity user {message.UserId}: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
            return;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        
        var rolesToRemove = currentRoles.Except(message.Roles).ToList();
        var rolesToAdd = message.Roles.Except(currentRoles).ToList();

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                _logger.LogWarning($"Failed to remove roles for Identity user {message.UserId}");
            }
        }

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                _logger.LogWarning($"Failed to add roles for Identity user {message.UserId}");
            }
        }

        _logger.LogInformation($"Successfully synchronized UserDetails for Identity user {message.UserId}.");
    }
}
