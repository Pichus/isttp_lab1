using Microsoft.AspNetCore.Identity;

using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Infrastructure.Data;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.Seeding.Seeders;

public class RoleSeeder : IRoleSeeder
{
    private readonly ApplicationDatabaseContext _applicationDatabaseContext;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleSeeder(ApplicationDatabaseContext applicationDatabaseContext,
        RoleManager<IdentityRole<Guid>> roleManager, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _applicationDatabaseContext = applicationDatabaseContext;
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SeedAsync()
    {
        var roleNames = Enum.GetValues<RoleName>();

        foreach (var roleName in roleNames)
        {
            var roleId = Guid.NewGuid();

            var roleExistsInIdentityModule = await _roleManager.FindByNameAsync(roleName.ToString()) is not null;

            if (!roleExistsInIdentityModule)
            {
                await PersistRoleInIdentityModuleAsync(roleName, roleId);
            }

            var roleExistsInApplicationModule = await _roleRepository.ExistsAsync(roleName);

            if (!roleExistsInApplicationModule)
            {
                await PersistRoleInApplicationModuleAsync(roleName, roleId);
            }
        }
    }

    private async Task PersistRoleInIdentityModuleAsync(RoleName roleName, Guid roleId)
    {
        var role = new IdentityRole<Guid> { Id = roleId, Name = roleName.ToString() };

        await _roleManager.CreateAsync(role);
    }

    private async Task PersistRoleInApplicationModuleAsync(RoleName roleName, Guid roleId)
    {
        var role = new Role { Id = roleId, CreatedAt = DateTimeOffset.UtcNow, Name = roleName };

        _roleRepository.Create(role);
        await _unitOfWork.SaveChangesAsync();
    }
}