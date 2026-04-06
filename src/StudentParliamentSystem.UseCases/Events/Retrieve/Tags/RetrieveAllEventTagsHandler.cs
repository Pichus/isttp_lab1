using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.Tags;

public class RetrieveAllEventTagsHandler
{
    private readonly IEventTagRepository _eventTagRepository;

    public RetrieveAllEventTagsHandler(IEventTagRepository eventTagRepository)
    {
        _eventTagRepository = eventTagRepository;
    }

    public async Task<IEnumerable<EventTag>> HandleAsync(RetrieveAllEventTags query)
    {
        return await _eventTagRepository.RetrieveAllAsync();
    }
}
