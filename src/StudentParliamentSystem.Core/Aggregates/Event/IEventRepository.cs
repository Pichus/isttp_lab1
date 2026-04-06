namespace StudentParliamentSystem.Core.Aggregates.Event;

public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
}
