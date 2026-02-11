using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Core.Aggregates.User;

public class User
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; init; }

    public ICollection<Role.Role> Roles { get; set; }
    public ICollection<Department.Department> Departments { get; set; }
    public ICollection<DepartmentMember> DepartmentMembers { get; set; }
    public ICollection<Event.Event> CreatedEvents { get; set; }
    public ICollection<EventRegistration> EventRegistrations { get; set; }
    public ICollection<OrganizationRequest.OrganizationRequest> OrganizationRequests { get; set; }
}