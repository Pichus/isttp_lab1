using FluentResults;

using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Departments.Update;

public class AddDepartmentMemberHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddDepartmentMemberHandler> _logger;

    public AddDepartmentMemberHandler(
        IUserRepository userRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddDepartmentMemberHandler> logger)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(AddDepartmentMember command)
    {
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
        if (department == null)
            return Result.Fail("Department not found");

        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
            return Result.Fail("User not found");

        if (!user.Departments.Any(d => d.Id == department.Id))
        {
            user.Departments.Add(department);
        }

        var memberRoleName = GetMemberRoleForDepartment(department.Name);
        if (memberRoleName != null)
        {
            if (!user.Roles.Any(r => r.Name == memberRoleName.Value))
            {
                var roleResult = await _roleRepository.GetByNameAsync(memberRoleName.Value);
                if (roleResult.IsSuccess)
                {
                    user.Roles.Add(roleResult.Value);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation($"User {user.Email} added to department {department.Name}");
        return Result.Ok();
    }

    private RoleName? GetMemberRoleForDepartment(string departmentName)
    {
        return departmentName switch
        {
            "Культурний" => RoleName.CulturalDepartmentMember,
            "Науковий" => RoleName.ScienceDepartmentMember,
            "Читалкадеп" => RoleName.CoworkingDepartmentMember,
            "Інформаційний" => RoleName.InformationDepartmentMember,
            _ => null
        };
    }
}