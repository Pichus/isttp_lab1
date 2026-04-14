using FluentResults;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Events.Register;

public class CancelEventRegistrationHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelEventRegistrationHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelEventRegistration command)
    {
        var @event = await _eventRepository.GetByIdWithRegistrationsAsync(command.EventId);
        if (@event == null)
            return Result.Fail("Event not found.");

        var registration = @event.Registrations.FirstOrDefault(r => r.UserId == command.UserId);
        if (registration == null)
            return Result.Fail("You are not registered for this event.");

        await _eventRepository.RemoveRegistrationAsync(registration);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
