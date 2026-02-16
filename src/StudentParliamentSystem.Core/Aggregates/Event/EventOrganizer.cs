namespace StudentParliamentSystem.Core.Aggregates.Event;

public class EventOrganizer
{
    public Guid Id { get; init; }
    public Guid EventId { get; init; }
    public Guid UserId { get; init; }
    public string? RoleDescription { get; set; }

    public User.User User { get; init; }
    public Event Event { get; init; }
}