namespace StudentParliamentSystem.Core.Aggregates.Event;

public class EventTag
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    
    public ICollection<Event> Events { get; set; }
}