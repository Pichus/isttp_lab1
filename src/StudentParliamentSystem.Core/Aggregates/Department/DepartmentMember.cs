namespace StudentParliamentSystem.Core.Aggregates.Department;

public class DepartmentMember
{
    public Guid UserId { get; init; }
    public Guid DepartmentId { get; init; }
    public Guid DepartmentRoleId { get; set; }

    public User.User User { get; init; }
    public Department Department { get; init; }
    public DepartmentRole DepartmentRole { get; set; }
}