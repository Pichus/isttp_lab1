namespace StudentParliamentSystem.Core.Abstractions;

public class BaseEntity<TId>
{
    public required TId Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}