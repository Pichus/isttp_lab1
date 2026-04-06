using FluentResults;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Events.Delete;

public class DeleteEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteEvent command)
    {
        var @event = await _eventRepository.GetByIdAsync(command.EventId);
        if (@event == null)
            return Result.Fail("Event not found");

        await _eventRepository.DeleteAsync(@event);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
