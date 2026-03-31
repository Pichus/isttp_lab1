using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.Seeding.Options;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.Seeding.Seeders;

public class InitialAdminUserSeeder : IInitialAdminUserSeeder
{
    private readonly InitialAdminAccountOptions _initialAdminAccountOptions;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;

    public InitialAdminUserSeeder(IOptions<InitialAdminAccountOptions> initialAdminAccountOptions,
        IRoleRepository roleRepository, UserManager<ApplicationUser> userManager, IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userManager = userManager;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _initialAdminAccountOptions = initialAdminAccountOptions.Value;
    }

    public async Task SeedAsync()
    {
        var userId = Guid.NewGuid();

        var userExistsInApplicationModule = await _userRepository.ExistsAsync(_initialAdminAccountOptions.Email);

        if (!userExistsInApplicationModule)
        {
            var applicationUser = new User
            {
                Id = userId,
                Email = _initialAdminAccountOptions.Email,
                FirstName = _initialAdminAccountOptions.FirstName,
                LastName = _initialAdminAccountOptions.LastName,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                Roles = (await FetchApplicationRolesAsync()).ToList()
            };

            await PersistUserInApplicationModule(applicationUser);
        }

        var userExistsInIdentityModule =
            await _userManager.FindByEmailAsync(_initialAdminAccountOptions.Email) is not null;

        if (!userExistsInIdentityModule)
        {
            var identityUser = new ApplicationUser
            {
                Id = userId,
                UserName = _initialAdminAccountOptions.Email,
                Email = _initialAdminAccountOptions.Email,
                FirstName = _initialAdminAccountOptions.FirstName,
                LastName = _initialAdminAccountOptions.LastName
            };

            await PersistUserInIdentityModule(identityUser);
        }
    }

    private async Task PersistUserInApplicationModule(User user)
    {
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task PersistUserInIdentityModule(ApplicationUser user)
    {
        var createUserResult = await _userManager.CreateAsync(user, _initialAdminAccountOptions.Password);

        if (!createUserResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unable to seed admin user to identity db:\n" +
                $"{string.Join("", createUserResult.Errors.Select(error => error.Description + "\n"))}");
        }

        var roles = _initialAdminAccountOptions.Roles.Select(role => role.ToString());

        var addRolesResult = await _userManager.AddToRolesAsync(user, roles);

        if (!addRolesResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unable to seed admin user to identity db:\n" +
                $"{string.Join("", addRolesResult.Errors.Select(error => error.Description + "\n"))}");
        }
    }

    private async Task<IEnumerable<Role>> FetchApplicationRolesAsync()
    {
        var roles = await _roleRepository.GetByNamesAsync(_initialAdminAccountOptions.Roles);

        if (roles.Count() != _initialAdminAccountOptions.Roles.Count())
        {
            throw new InvalidOperationException("Admin roles specified in the config do not exist");
        }

        return roles;
    }
}