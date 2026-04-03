using Microsoft.EntityFrameworkCore;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Role;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.Departments;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDatabaseContext _dbContext;

    public DepartmentRepository(ApplicationDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DepartmentPreview>> RetrieveAllPreviewsAsync()
    {
        var departments = await _dbContext.Departments.ToListAsync();
        var previews = new List<DepartmentPreview>();

        foreach (var department in departments)
        {
            var (headRole, memberRole) = GetRolesForDepartment(department.Name);

            string? headName = null;
            if (headRole != null)
            {
                var headUser = await _dbContext.Users
                    .Where(u => u.Roles.Any(r => r.Name == headRole))
                    .FirstOrDefaultAsync();
                
                if (headUser != null)
                {
                    headName = $"{headUser.FirstName} {headUser.LastName}";
                }
            }

            int memberCount = 0;
            if (headRole != null || memberRole != null)
            {
                memberCount = await _dbContext.Users
                    .Where(u => u.Roles.Any(r => r.Name == headRole || r.Name == memberRole))
                    .CountAsync();
            }

            previews.Add(new DepartmentPreview(
                department.Id,
                department.Name,
                department.Description,
                headName,
                memberCount
            ));
        }

        return previews;
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

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id);
    }
}
