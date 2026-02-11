namespace StudentParliamentSystem.Core.Aggregates.Event;

public class EventRegistration
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid EventId { get; init; }
    public DateTime RegisteredAtUtc { get; init; }

    public User.User User { get; set; }
    public Event Event { get; set; }
}