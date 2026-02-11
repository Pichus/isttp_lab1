namespace StudentParliamentSystem.Core.Aggregates.Department;

public class DepartmentRole
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    
    public ICollection<DepartmentMember> DepartmentMembers { get; set; }
}