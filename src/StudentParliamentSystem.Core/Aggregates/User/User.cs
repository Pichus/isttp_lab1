using StudentParliamentSystem.Core.Aggregates.Department;
using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.Core.Aggregates.User;

public class User
{
    public Guid Id { get; init; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTimeOffset CreatedAtUtc { get; init; }

    public ICollection<Role.Role> Roles { get; set; } = [];
    public ICollection<Department.Department> Departments { get; set; } = [];
    public ICollection<DepartmentMember> DepartmentMembers { get; set; } = [];
    public ICollection<Event.Event> CreatedEvents { get; set; } = [];
    public ICollection<EventRegistration> EventRegistrations { get; set; } = [];
    public ICollection<OrganizationRequest.OrganizationRequest> OrganizationRequests { get; set; } = [];

    public static User Create(Guid id, string email, string firstName, string lastName)
    {
        return new User
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}