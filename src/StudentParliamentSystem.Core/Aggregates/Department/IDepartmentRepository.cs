namespace StudentParliamentSystem.Core.Aggregates.Department;

public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentPreview>> RetrieveAllPreviewsAsync();
    Task<Department?> GetByIdAsync(Guid id);
}