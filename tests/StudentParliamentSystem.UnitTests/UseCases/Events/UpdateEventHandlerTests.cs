using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Events.Update;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class UpdateEventHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IEventTagRepository> _tagRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly UpdateEventHandler _sut;

    public UpdateEventHandlerTests()
    {
        _sut = new UpdateEventHandler(_eventRepo.Object, _tagRepo.Object, _uow.Object);
    }

    private static UpdateEvent MakeCommand(Guid eventId, string[]? tags = null) => new(
        eventId,
        "New Title",
        "New Description",
        "New Location",
        DateTime.UtcNow.AddDays(1),
        DateTime.UtcNow.AddDays(1).AddHours(2),
        Guid.NewGuid(),
        100,
        true,
        tags ?? [],
        Guid.NewGuid()
    );

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFail()
    {
        var eventId = Guid.NewGuid();
        _eventRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync((Event?)null);

        var result = await _sut.HandleAsync(MakeCommand(eventId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Event not found");
    }

    [Fact]
    public async Task HandleAsync_ValidUpdate_UpdatesPropertiesAndSaves()
    {
        var eventId = Guid.NewGuid();
        var @event = new Event { Id = eventId, Tags = new List<EventTag>() };
        _eventRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(@event);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = MakeCommand(eventId);
        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        @event.Title.Should().Be(command.Title);
        @event.Description.Should().Be(command.Description);
        @event.Location.Should().Be(command.Location);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateTags_ReplacesOldTagsWithNewOnes()
    {
        var eventId = Guid.NewGuid();
        var oldTag = new EventTag { Id = Guid.NewGuid(), Name = "old" };
        var @event = new Event { Id = eventId, Tags = new List<EventTag> { oldTag } };
        
        var existingTag = new EventTag { Id = Guid.NewGuid(), Name = "existing" };
        _eventRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(@event);
        _tagRepo.Setup(r => r.GetByNameAsync("existing")).ReturnsAsync(existingTag);
        _tagRepo.Setup(r => r.GetByNameAsync("new")).ReturnsAsync((EventTag?)null);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(MakeCommand(eventId, ["existing", "new"]));

        result.IsSuccess.Should().BeTrue();
        @event.Tags.Should().NotContain(oldTag);
        @event.Tags.Should().Contain(existingTag);
        @event.Tags.Any(t => t.Name == "new").Should().BeTrue();
        _tagRepo.Verify(r => r.AddAsync(It.Is<EventTag>(t => t.Name == "new")), Times.Once);
    }
}
