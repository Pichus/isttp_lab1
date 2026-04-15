using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Events.Register;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class CancelEventRegistrationHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CancelEventRegistrationHandler _sut;

    public CancelEventRegistrationHandlerTests()
    {
        _sut = new CancelEventRegistrationHandler(_eventRepo.Object, _uow.Object);
    }

    private static Event MakeEvent(List<EventRegistration>? registrations = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Test",
        Description = "",
        Location = "Loc",
        IsPublished = true,
        StartTimeUtc = DateTime.UtcNow.AddDays(1),
        EndTimeUtc = DateTime.UtcNow.AddDays(1).AddHours(2),
        CreatedByUserId = Guid.NewGuid(),
        DepartmentId = Guid.NewGuid(),
        CreatedAtUtc = DateTime.UtcNow,
        Tags = new List<EventTag>(),
        Registrations = registrations ?? new List<EventRegistration>()
    };

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFail()
    {
        var eventId = Guid.NewGuid();
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(eventId)).ReturnsAsync((Event?)null);

        var result = await _sut.HandleAsync(new CancelEventRegistration(eventId, Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Event not found");
    }

    [Fact]
    public async Task HandleAsync_NotRegistered_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var @event = MakeEvent();
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);

        var result = await _sut.HandleAsync(new CancelEventRegistration(@event.Id, userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("not registered");
    }

    [Fact]
    public async Task HandleAsync_Registered_CancelsAndSaves()
    {
        var userId = Guid.NewGuid();
        var reg = new EventRegistration { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid(), RegisteredAtUtc = DateTime.UtcNow };
        var @event = MakeEvent(registrations: [reg]);
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);
        _eventRepo.Setup(r => r.RemoveRegistrationAsync(reg)).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(new CancelEventRegistration(@event.Id, userId));

        result.IsSuccess.Should().BeTrue();
        _eventRepo.Verify(r => r.RemoveRegistrationAsync(reg), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
