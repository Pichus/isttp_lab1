using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.Published;

public class RetrievePublishedEventsHandler
{
    private readonly IEventRepository _eventRepository;

    public RetrievePublishedEventsHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PagedResult<EventPreview>> HandleAsync(RetrievePublishedEvents query)
    {
        return await _eventRepository.RetrievePublishedAsync(
            query.PageNumber,
            query.PageSize,
            query.Query,
            query.Tag,
            query.SortOrder
        );
    }
}
