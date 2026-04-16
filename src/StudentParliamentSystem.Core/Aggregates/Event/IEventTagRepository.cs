namespace StudentParliamentSystem.Core.Aggregates.Event;

public interface IEventTagRepository
{
    Task<EventTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventTag>> RetrieveAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(EventTag eventTag, CancellationToken cancellationToken = default);
}