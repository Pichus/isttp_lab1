using FluentResults;

using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Events.Update;

public class UpdateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventTagRepository _eventTagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventHandler(
        IEventRepository eventRepository,
        IEventTagRepository eventTagRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _eventTagRepository = eventTagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateEvent command)
    {
        var @event = await _eventRepository.GetByIdAsync(command.EventId);
        if (@event == null)
            return Result.Fail("Event not found");




        @event.Title = command.Title;
        @event.Description = command.Description;
        @event.Location = command.Location;
        @event.StartTimeUtc = command.StartTimeUtc;
        @event.EndTimeUtc = command.EndTimeUtc;
        @event.DepartmentId = command.DepartmentId;
        @event.MaxParticipants = command.MaxParticipants;
        @event.IsPublished = command.IsPublished;

        @event.Tags.Clear();

        foreach (var tagName in command.Tags)
        {
            var trimmedName = tagName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName)) continue;

            var existingTag = await _eventTagRepository.GetByNameAsync(trimmedName);
            if (existingTag != null)
            {
                if (!@event.Tags.Any(t => t.Id == existingTag.Id))
                    @event.Tags.Add(existingTag);
            }
            else
            {
                var newTag = new EventTag
                {
                    Id = Guid.NewGuid(),
                    Name = trimmedName
                };
                await _eventTagRepository.AddAsync(newTag);
                @event.Tags.Add(newTag);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}