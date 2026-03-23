using StudentParliamentSystem.Core.Abstractions;

namespace StudentParliamentSystem.Core.Aggregates.Role;

public class Role : BaseEntity<Guid>
{
    public RoleName Name { get; set; }

    public ICollection<User.User> Users { get; set; } = [];
}