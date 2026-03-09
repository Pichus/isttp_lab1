using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Infrastructure.Identity.Data.Entities;
using StudentParliamentSystem.Infrastructure.Identity.Options;

namespace StudentParliamentSystem.Infrastructure.Identity.Data.Seeders;

public class IdentityDataSeeder : IIdentityDataSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly StarterAdminAccountOptions _starterAdminAccountOptions;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager, IOptions<StarterAdminAccountOptions> starterAdminAccountOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _starterAdminAccountOptions = starterAdminAccountOptions.Value;
    }

    public async Task SeedAsync()
    {
        var roleNames = new[] { RoleNameConstants.SuperAdminRoleName };
        await CreateAndPersistRolesAsync(roleNames);

        await CreateAndPersistSuperAdminUserAsync();
    }

    private async Task CreateAndPersistRolesAsync(IEnumerable<string> roleNames)
    {
        foreach (var roleName in roleNames)
        {
            var role = CreateRole(roleName);
            await PersistRoleAsync(role);
        }
    }

    private IdentityRole<Guid> CreateRole(string roleName)
    {
        return new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.Normalize() };
    }

    private async Task PersistRoleAsync(IdentityRole<Guid> role)
    {
        if (role.Name is null)
        {
            throw new InvalidOperationException("Role name can't be null while seeding the database");
        }

        var roleExists = await _roleManager.RoleExistsAsync(role.Name);

        if (!roleExists)
        {
            await _roleManager.CreateAsync(role);
        }
    }

    private async Task CreateAndPersistSuperAdminUserAsync()
    {
        var superAdminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = _starterAdminAccountOptions.Name,
            Email = _starterAdminAccountOptions.Email,
            EmailConfirmed = true
        };

        await CreateAndPersistUserAsync(superAdminUser, _starterAdminAccountOptions.Password);

        var createdUser = await _userManager.FindByEmailAsync(_starterAdminAccountOptions.Email);

        if (createdUser is null)
        {
            throw new InvalidOperationException("Created user can't be null while seeding the database");
        }

        var createdUserHasSuperAdminRole =
            await _userManager.IsInRoleAsync(createdUser, RoleNameConstants.SuperAdminRoleName);

        if (createdUserHasSuperAdminRole)
        {
            return;
        }

        await _userManager.AddToRoleAsync(createdUser, RoleNameConstants.SuperAdminRoleName);
    }

    private async Task CreateAndPersistUserAsync(ApplicationUser user, string password)
    {
        if (user.Email is null)
        {
            throw new InvalidOperationException("User email can't be null while seeding the database");
        }

        var userFromDb = await _userManager.FindByEmailAsync(user.Email);

        var userExists = userFromDb is not null;

        if (userExists)
        {
            return;
        }

        await _userManager.CreateAsync(user, password);
    }
}