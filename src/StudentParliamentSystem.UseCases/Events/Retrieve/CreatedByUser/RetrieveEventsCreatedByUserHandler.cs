using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.CreatedByUser;

public class RetrieveEventsCreatedByUserHandler
{
    private readonly IEventRepository _eventRepository;

    public RetrieveEventsCreatedByUserHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Event>> HandleAsync(RetrieveEventsCreatedByUser query)
    {
        return await _eventRepository.GetByCreatedByUserAsync(query.UserId);
    }
}
