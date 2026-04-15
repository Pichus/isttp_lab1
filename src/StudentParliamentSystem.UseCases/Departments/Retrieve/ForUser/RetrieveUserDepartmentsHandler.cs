using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.UseCases.Users.Retrieve.ById;

using Wolverine;

namespace StudentParliamentSystem.UseCases.Departments.Retrieve.ForUser;

public class RetrieveUserDepartmentsHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMessageBus _bus;

    public RetrieveUserDepartmentsHandler(IDepartmentRepository departmentRepository, IMessageBus bus)
    {
        _departmentRepository = departmentRepository;
        _bus = bus;
    }

    public async Task<IEnumerable<DepartmentPreview>> HandleAsync(RetrieveUserDepartments query)
    {
        var userResult = await _bus.InvokeAsync<FluentResults.Result<StudentParliamentSystem.Core.Aggregates.User.User>>(new RetrieveUserById(query.UserId));

        var allDepartments = await _departmentRepository.RetrieveAllPreviewsAsync();

        if (userResult.IsFailed)
        {
            return Enumerable.Empty<DepartmentPreview>();
        }

        var user = userResult.Value;
        var userRoleNames = user.Roles.Select(r => r.Name).ToHashSet();


        if (userRoleNames.Contains(RoleName.SuperAdmin))
        {
            return allDepartments;
        }

        var allowedDepartments = new List<DepartmentPreview>();

        foreach (var dept in allDepartments)
        {
            var (headRole, memberRole) = GetRolesForDepartment(dept.Name);
            if ((headRole.HasValue && userRoleNames.Contains(headRole.Value)) ||
                (memberRole.HasValue && userRoleNames.Contains(memberRole.Value)))
            {
                allowedDepartments.Add(dept);
            }
        }

        return allowedDepartments;
    }

    private (RoleName? headRole, RoleName? memberRole) GetRolesForDepartment(string departmentName)
    {
        return departmentName switch
        {
            "Культурний" => (RoleName.HeadOfCulturalDepartment, RoleName.CulturalDepartmentMember),
            "Науковий" => (RoleName.HeadOfScienceDepartment, RoleName.ScienceDepartmentMember),
            "Читалкадеп" => (RoleName.HeadOfCoworkingDepartment, RoleName.CoworkingDepartmentMember),
            "Інформаційний" => (RoleName.HeadOfInformationDepartment, RoleName.InformationDepartmentMember),
            _ => (null, null)
        };
    }
}