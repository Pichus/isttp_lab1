using FluentResults;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Events.Register;

public class RegisterForEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterForEventHandler(IEventRepository eventRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RegisterForEvent command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
            return Result.Fail("Your account profile was not found in the system.");

        var @event = await _eventRepository.GetByIdWithRegistrationsAsync(command.EventId);
        if (@event == null)
            return Result.Fail("Event not found.");

        if (!@event.IsPublished)
            return Result.Fail("Cannot register for an unpublished event.");

        if (@event.Registrations.Any(r => r.UserId == command.UserId))
            return Result.Fail("You are already registered for this event.");

        if (@event.MaxParticipants.HasValue && @event.Registrations.Count >= @event.MaxParticipants.Value)
            return Result.Fail("This event is full.");

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid(),
            EventId = command.EventId,
            UserId = command.UserId,
            RegisteredAtUtc = DateTime.UtcNow
        };

        await _eventRepository.AddRegistrationAsync(registration);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}

