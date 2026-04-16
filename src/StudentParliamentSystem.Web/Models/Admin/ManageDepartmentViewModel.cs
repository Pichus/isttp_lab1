using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.Api.Models.Admin;

public class ManageDepartmentViewModel
{
    public DepartmentPreview Department { get; set; }
    public PagedResult<UserPreview> Users { get; set; }
    public PagedResult<EventPreview> Events { get; set; }
}