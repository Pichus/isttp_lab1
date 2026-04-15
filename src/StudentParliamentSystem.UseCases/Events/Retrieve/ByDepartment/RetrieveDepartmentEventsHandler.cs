using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.ByDepartment;

public class RetrieveDepartmentEventsHandler
{
    private readonly IEventRepository _eventRepository;

    public RetrieveDepartmentEventsHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PagedResult<EventPreview>> HandleAsync(RetrieveDepartmentEvents query)
    {
        return await _eventRepository.RetrieveByDepartmentAsync(
            query.DepartmentId,
            query.PageNumber,
            query.PageSize,
            query.Query
        );
    }
}