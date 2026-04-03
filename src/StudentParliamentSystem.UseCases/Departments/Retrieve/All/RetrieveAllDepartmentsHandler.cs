using StudentParliamentSystem.Core.Aggregates.Department;

namespace StudentParliamentSystem.UseCases.Departments.Retrieve.All;

public class RetrieveAllDepartmentsHandler
{
    private readonly IDepartmentRepository _departmentRepository;

    public RetrieveAllDepartmentsHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<DepartmentPreview>> HandleAsync(RetrieveAllDepartments query)
    {
        return await _departmentRepository.RetrieveAllPreviewsAsync();
    }
}
