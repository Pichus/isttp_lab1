namespace StudentParliamentSystem.Core.Aggregates.Department;

public class Department
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<User.User> Users { get; set; }
    public ICollection<DepartmentMember> DepartmentMembers { get; set; }
}