using FluentResults;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.ById;

public class RetrieveEventByIdHandler
{
    private readonly IEventRepository _eventRepository;

    public RetrieveEventByIdHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Result<Event>> HandleAsync(RetrieveEventById query)
    {
        var @event = await _eventRepository.GetByIdAsync(query.Id);

        if (@event == null)
            return Result.Fail<Event>("Event not found");

        return Result.Ok(@event);
    }
}
