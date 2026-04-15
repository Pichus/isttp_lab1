using FluentResults;

using Microsoft.Extensions.Logging;

using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Departments.Update;

public class ChangeDepartmentHeadHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeDepartmentHeadHandler> _logger;

    public ChangeDepartmentHeadHandler(
        IUserRepository userRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeDepartmentHeadHandler> logger)
    {
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ChangeDepartmentHead command)
    {
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
        if (department == null) return Result.Fail("Department not found");

        var newHeadUser = await _userRepository.GetByEmailAsync(command.Email);
        if (newHeadUser == null) return Result.Fail("User not found");

        var headRoleName = GetHeadRoleForDepartment(department.Name);
        if (headRoleName == null) return Result.Fail("Department has no head role mapping");

        var roleResult = await _roleRepository.GetByNameAsync(headRoleName.Value);
        if (roleResult.IsFailed) return Result.Fail("Head role not found in system");

        var headRole = roleResult.Value;


        var existingHeads = await _userRepository.GetUsersByRoleAsync(headRole.Id);

        foreach (var existingHead in existingHeads)
        {
            var roleToRemove = existingHead.Roles.FirstOrDefault(r => r.Id == headRole.Id);
            if (roleToRemove != null)
            {
                existingHead.Roles.Remove(roleToRemove);
            }
        }


        if (!newHeadUser.Roles.Any(r => r.Id == headRole.Id))
        {
            newHeadUser.Roles.Add(headRole);
        }


        if (!newHeadUser.Departments.Any(d => d.Id == department.Id))
        {
            newHeadUser.Departments.Add(department);
        }


        var memberRoleName = GetMemberRoleForDepartment(department.Name);
        if (memberRoleName != null && !newHeadUser.Roles.Any(r => r.Name == memberRoleName.Value))
        {
            var memberRoleResult = await _roleRepository.GetByNameAsync(memberRoleName.Value);
            if (memberRoleResult.IsSuccess)
            {
                newHeadUser.Roles.Add(memberRoleResult.Value);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation($"User {newHeadUser.Email} is now the head of {department.Name}");

        return Result.Ok();
    }

    private RoleName? GetHeadRoleForDepartment(string departmentName)
    {
        return departmentName switch
        {
            "Культурний" => RoleName.HeadOfCulturalDepartment,
            "Науковий" => RoleName.HeadOfScienceDepartment,
            "Читалкадеп" => RoleName.HeadOfCoworkingDepartment,
            "Інформаційний" => RoleName.HeadOfInformationDepartment,
            _ => null
        };
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