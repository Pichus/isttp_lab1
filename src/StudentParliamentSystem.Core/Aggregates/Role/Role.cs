namespace StudentParliamentSystem.Core.Aggregates.Role;

public class Role
{
    public Guid Id { get; init; }
    public string Name { get; set; }

    public ICollection<User.User> Users { get; set; }
}