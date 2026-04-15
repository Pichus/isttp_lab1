using FluentResults;

using StudentParliamentSystem.Core.Aggregates.Event;

namespace StudentParliamentSystem.UseCases.Events.Retrieve.ById;

public class RetrieveEventByIdWithRegistrationsHandler
{
    private readonly IEventRepository _eventRepository;

    public RetrieveEventByIdWithRegistrationsHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Result<Event>> HandleAsync(RetrieveEventByIdWithRegistrations query)
    {
        var @event = await _eventRepository.GetByIdWithRegistrationsAsync(query.Id);

        if (@event == null)
            return Result.Fail<Event>("Event not found");

        return Result.Ok(@event);
    }
}