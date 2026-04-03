namespace StudentParliamentSystem.Core.Aggregates.User;

public class UserPreview
{
    public Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public IEnumerable<string> Departments { get; init; } = [];
    public IEnumerable<string> Roles { get; init; } = [];
}