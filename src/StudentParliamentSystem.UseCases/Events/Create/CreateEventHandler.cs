using FluentResults;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.Events.Create;

public class CreateEventHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventTagRepository _eventTagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventHandler(
        IEventRepository eventRepository,
        IEventTagRepository eventTagRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _eventTagRepository = eventTagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CreateEvent command)
    {
        var @event = new Event
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Location = command.Location,
            StartTimeUtc = command.StartTimeUtc,
            EndTimeUtc = command.EndTimeUtc,
            DepartmentId = command.DepartmentId,
            MaxParticipants = command.MaxParticipants,
            IsPublished = command.IsPublished,
            CreatedByUserId = command.CreatedByUserId,
            CreatedAtUtc = DateTime.UtcNow,
            Tags = new List<EventTag>()
        };

        foreach (var tagName in command.Tags)
        {
            var trimmedName = tagName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName)) continue;

            var existingTag = await _eventTagRepository.GetByNameAsync(trimmedName);
            if (existingTag != null)
            {
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

        await _eventRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
