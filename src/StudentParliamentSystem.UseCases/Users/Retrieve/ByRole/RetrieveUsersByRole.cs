using FluentResults;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.UseCases.Users.Retrieve.ByRole;

public record RetrieveUsersByRole(RoleName RoleName);

public class RetrieveUsersByRoleHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public RetrieveUsersByRoleHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<IEnumerable<UserPreview>>> HandleAsync(RetrieveUsersByRole query)
    {
        var roleResult = await _roleRepository.GetByNameAsync(query.RoleName);
        if (roleResult.IsFailed)
        {
            return Result.Fail("Role not found");
        }

        var users = await _userRepository.GetUsersByRoleAsync(roleResult.Value.Id);
        
        var userPreviews = users.Select(u => new UserPreview
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Roles = u.Roles.Select(r => r.Name.ToString()),
            Departments = u.Departments.Select(d => d.Name)
        });

        return Result.Ok(userPreviews);
    }
}
