using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Events.Delete;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class DeleteEventHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly DeleteEventHandler _sut;

    public DeleteEventHandlerTests()
    {
        _sut = new DeleteEventHandler(_eventRepo.Object, _uow.Object);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFail()
    {
        var eventId = Guid.NewGuid();
        _eventRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync((Event?)null);

        var result = await _sut.HandleAsync(new DeleteEvent(eventId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Event not found");
    }

    [Fact]
    public async Task HandleAsync_ValidDelete_DeletesAndSaves()
    {
        var eventId = Guid.NewGuid();
        var @event = new Event { Id = eventId };
        _eventRepo.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(@event);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(new DeleteEvent(eventId));

        result.IsSuccess.Should().BeTrue();
        _eventRepo.Verify(r => r.DeleteAsync(@event), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
